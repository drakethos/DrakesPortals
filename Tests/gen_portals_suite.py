"""Writes the DrakesTestLab suite for DrakesPortals into Tests/suites/DrakesPortals (run: python gen_portals_suite.py).
The suite is generated: do not edit the JSON by hand. Tests/ is never packaged.
Hooks live in ../Testing/PortalsTestHooks.cs (Debug builds only)."""
import json, os, shutil

OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'suites', 'DrakesPortals')
GUID = 'com.drakemods.portals'
NOERR = {'check': 'noErrors', 'source': 'DrakesPortals'}


def hook(name, expect=None, label=None, contains=None, not_contains=None, stop=False, **args):
    s = {'do': 'hook', 'name': 'portals.' + name}
    if args:
        s['args'] = args
    for k, v in (('expect', expect), ('contains', contains), ('notContains', not_contains), ('label', label)):
        if v is not None:
            s[k] = v
    if stop:
        s['stop'] = True
    return s


def wait(seconds=0.5):
    return {'do': 'wait', 'seconds': seconds}


def spawn_portal(tag, dx, dz):
    return {'do': 'spawn', 'prefab': 'portal_wood', 'dx': dx, 'dz': dz, 'as': tag}


def pair(tag='pair'):
    """Two portals with the same tag; the game links them a moment later."""
    return [
        spawn_portal('a', 3, 3),
        spawn_portal('b', -3, 3),
        wait(0.5),
        hook('setTag', tag=tag, target='a'),
        hook('setTag', tag=tag, target='b'),
        wait(5),
        hook('linked', True, 'portal A is linked', target='a'),
        hook('linked', True, 'portal B is linked', target='b', stop=True),
    ]


def set_dir(direction, target='a'):
    return [hook('setDirection', True, f'ask the server: {direction}', direction=direction, target=target), wait(1.5)]


def test(code, title, steps, description=''):
    return {'name': f'{code} {title}', 'description': description, 'steps': steps, 'asserts': [NOERR]}


tests = []

tests.append(test('PT-01', 'A new portal is a normal two-way portal',
    pair() + [
        hook('direction', 'TwoWay', 'A is two-way', target='a'),
        hook('direction', 'TwoWay', 'B is two-way', target='b'),
        hook('blocked', False, 'stepping into A is allowed', target='a'),
        hook('blocked', False, 'stepping into B is allowed', target='b'),
    ]))

tests.append(test('PT-02', 'One-way needs a linked pair',
    [
        spawn_portal('solo', 3, 3), wait(0.5),
        hook('setTag', tag='lonely', target='solo'), wait(2),
        hook('linked', False, 'nothing to link to', target='solo'),
        hook('offered', False, 'Entry is not offered', direction='Entry', target='solo'),
        hook('offered', False, 'Exit is not offered', direction='Exit', target='solo'),
        hook('setDirection', True, 'a request can still be sent', direction='Entry', target='solo'),
        wait(1.5),
        hook('direction', 'TwoWay', 'but the server refuses it', target='solo'),
    ]))

tests.append(test('PT-03', 'Entry on A makes B the exit',
    pair() + set_dir('Entry') + [
        hook('direction', 'Entry', 'A is the entry', target='a'),
        hook('direction', 'Exit', 'B became the exit', target='b'),
        hook('blocked', False, 'A can be stepped into', target='a'),
        hook('blocked', True, 'B refuses players', target='b'),
    ]))

tests.append(test('PT-04', 'Exit on A makes B the entry',
    pair() + set_dir('Exit') + [
        hook('direction', 'Exit', 'A is the exit', target='a'),
        hook('direction', 'Entry', 'B became the entry', target='b'),
        hook('blocked', True, 'A refuses players', target='a'),
        hook('blocked', False, 'B can be stepped into', target='b'),
    ]))

tests.append(test('PT-05', 'Two-way puts both ends back',
    pair() + set_dir('Entry') + set_dir('TwoWay') + [
        hook('direction', 'TwoWay', 'A is two-way again', target='a'),
        hook('direction', 'TwoWay', 'B is two-way again', target='b'),
        hook('blocked', False, 'B can be stepped into again', target='b'),
    ]))

tests.append(test('PT-06', 'The hover text says which way a portal goes',
    pair() + set_dir('Entry') + [
        hook('hover', None, 'A says it is the entry', contains='One-way', target='a'),
        hook('hover', None, 'B says exit only', contains='Exit only', target='b'),
        hook('hover', None, 'the admin sees the menu key', contains='Shift+E', target='a'),
    ]))

tests.append(test('PT-07', 'Shift+E opens the direction menu and a click sets the direction',
    pair() + [
        {'do': 'face', 'target': 'a'},
        {'do': 'interact', 'target': 'a', 'alt': True},
        wait(0.5),
        {'do': 'check', 'check': 'uiVisible', 'name': 'Portal direction', 'expect': True, 'label': 'the menu opened'},
        {'do': 'check', 'check': 'uiVisible', 'name': 'One-way: this is the Entry', 'exact': True, 'expect': True, 'label': 'Entry is offered'},
        {'do': 'check', 'check': 'uiVisible', 'name': 'Two-way (normal portal)', 'exact': True, 'expect': False, 'label': 'the current state is not offered'},
        {'do': 'click', 'button': 'One-way: this is the Entry'},
        wait(1.5),
        hook('direction', 'Entry', 'A is the entry', target='a'),
        hook('direction', 'Exit', 'B is the exit', target='b'),
        {'do': 'closeUi'},
    ]))

tests.append(test('PT-08', 'Switched off, portals are vanilla',
    pair() + set_dir('Entry') + [
        {'do': 'setConfig', 'values': {'01 Admin/Enabled': 'false'}},
        wait(0.5),
        hook('blocked', False, 'the exit no longer refuses players', target='b'),
        hook('canManageHere', False, 'nobody can open the menu'),
        hook('hover', None, 'the hover text is vanilla', not_contains='One-way', target='a'),
        {'do': 'face', 'target': 'a'},
        {'do': 'interact', 'target': 'a', 'alt': True},
        wait(0.4),
        {'do': 'check', 'check': 'uiVisible', 'name': 'Portal direction', 'expect': False, 'label': 'Shift+E opens nothing'},
        {'do': 'closeUi'},
    ]))

tests.append(test('PT-09', 'ManageAccess: admin only or everyone',
    [
        {'do': 'setConfig', 'values': {'02 Portals/ManageAccess': 'AdminOnly'}},
        hook('canManage', False, 'AdminOnly: a normal player cannot', admin=False),
        hook('canManage', True, 'AdminOnly: an admin can', admin=True),
        {'do': 'setConfig', 'values': {'02 Portals/ManageAccess': 'Everyone'}},
        hook('canManage', True, 'Everyone: a normal player can', admin=False),
        hook('canManage', True, 'Everyone: an admin can', admin=True),
    ]))

tests.append(test('PT-10', 'The direction survives a relog',
    pair() + set_dir('Entry') + [
        {'do': 'relog', 'timeout': 120},
        wait(3),
        hook('direction', 'Entry', 'A is still the entry', target='a'),
        hook('direction', 'Exit', 'B is still the exit', target='b'),
    ]))

tests.append(test('PT-11', 'The glow shows the direction',
    pair() + set_dir('Entry') + [
        wait(1),
        hook('glow', '89,217,255', 'the entry glows the entry colour', target='a'),
        hook('glow', '255,140,51', 'the exit glows the exit colour', target='b'),
    ] + set_dir('TwoWay') + [
        wait(1),
        hook('glow', None, 'back to the normal glow', not_contains='255,140,51', target='b'),
    ]))

suite = {
    'name': 'DrakesPortals',
    'description': 'One-way portals: linked pairs, Entry / Exit, the Shift+E menu, who may change them, and what the portal shows.',
    'requireMods': [{'guid': GUID, 'minVersion': '0.1.0'}],
    'configOverrides': [
        {'guid': GUID, 'section': '01 Admin', 'key': 'Enabled', 'value': 'true'},
        {'guid': GUID, 'section': '02 Portals', 'key': 'ManageAccess', 'value': 'AdminOnly'},
        {'guid': GUID, 'section': '03 Display', 'key': 'ShowDirectionTint', 'value': 'true'},
    ],
    'setup': {'timeOfDay': 0.5, 'freezeTime': 1, 'god': 1, 'noBuild': 'off', 'hideUnlockPopups': 1},
    'resetBetweenTests': True,
}

if os.path.isdir(OUT):
    shutil.rmtree(OUT)
os.makedirs(OUT)
with open(os.path.join(OUT, 'suite.json'), 'w') as f:
    json.dump(suite, f, indent=2)
for t in tests:
    code = t['name'].split(' ')[0]
    with open(os.path.join(OUT, code + '.json'), 'w') as f:
        json.dump(t, f, indent=2)
print(f'{len(tests)} tests written to {OUT}')
