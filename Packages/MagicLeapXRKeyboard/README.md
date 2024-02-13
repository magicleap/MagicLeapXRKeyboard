# Magic Leap XR Keyboard
A keyboard that can be used in any project that supports Unity's XR Interaction Toolkit. Demo scene can be found in `Assets/MagicLeap/Runtime/KeyboardExample/Scenes/XRKeyboardExample.unity`

## How to import

Add the following git url to the [**Unity Package Manager**](https://docs.unity3d.com/Manual/upm-ui-giturl.html): `https://github.com/magicleap/MagicLeapXRKeyboard.git?path=/Packages/MagicLeapXRKeyboard`


## Scene Setup

To have the same interactions as in the example scene, make the following changes:
1. Select the **Game Controller** GameObject from the **XR Rig**
2. Disable  `Force Grab` from the **XR Ray Interactor**
2. In the same component, enable `Keep Selected Target Valid`

## Standard Layouts 

![Image of XR Keyboard Layout](Docs/DefaultLayout.png)
![Image of custom XR Keyboard Layout](Docs/CustomLayout.png)
**Learn how to create cusom layouts [here](#editing-layout)**


## Getting Started
Make sure to set the input option in **Player Settings** to `new`.

### Keyboard
- Place the Keyboard prefab from `Packages/Magic Leap XR Keyboard/Runtime/Prefabs/Keyboard Manager.prefab` into the scene

### Set Up Input Fields
- Add the script `TMPInputFieldTextReceiver.cs` onto any **TextMeshPro Input Field** you want to use with the Keyboard

## Structure
The Keyboard prefab is structured in the following way:

- `KeyboardManager.cs` -  Allows users to toggle the **Keyboard** on and off.
- `Keyboard.cs` - Manages multiple **Keyboard Layouts** and controls modifier key presses like Shift  and Caps Lock
- `KeyboardLayout.cs`  - groups the **Keys** and **Keyboard Rows** and communicates to the **Keyboard Builder** to generate a new layout if needed.
- `KeyboardBuilder.cs` - consumes JSON data to create the **Keyboard Layout** objects. The script requires that you assign the **Keyboard Key** and **Keyboard Row** prefab. 
- `KeyboardKey.cs` - Controls the visual key graphics and input

### Editing Layout 

To edit the layout:
1. Create or navigate to an existing **Keyboard Layout**.
2. Edit the **Keyboard Layout Data** inside of the **Keyboard Builder ** component. 
3. Press **Regenerate Keyboard** to update the keyboard layout in the scene.
4. When you are finished, modify the **Layout ID** and **description**.
5.  Press **Write New Json** .to save the data to the **Streaming Assets** folder. 
**Note:** to toggle between Panels with a key press, assign the key's keycode as the **Panel ID**

### Editing Visuals
1.  Navigate to an existing **Keyboard Layout**.
2. Modify the Key prefab in the **Keyboard Builder** component. 
3. Press **Regenerate Keyboard** to update the keyboard layout in the scene.

## Unicode and Special Characters
Magic Leap XR Keyboard uses the following fonts to support unicode characters:
|Font| Link |
|--|--|
| Material Symbols |https://github.com/google/material-design-icons |
| Quivira  | http://www.quivira-font.com/characters.php|

#### Shift
The shift label is appended with `_NEUTRAL` `_SHIFT`  and `_CAPS` at runtime depending on the shift state. A key with the label **`SHIFT`** will use the following unicodes (they can be changed in `Packages/Magic Leap XR Keyboard/Runtime/Scripts/Keyboard/DataModels/KeyboardCollections.cs`)
|Label Name | Value|
|--|--|
|SHIFT_NEUTRAL|\ue5f2|
|SHIFT_SHIFT|\uf7ae|
|SHIFT_CAPS|\ue318|
|SHIFT|\ue5f2|

#### Other Special Labels 
There are also other special labels to make it easier to distinguish in the editor
|Label Name | Value|
|--|--|
|NUMBERPAD|\uf045|
|BACKSPACE|\ue14a|
|SPACE| |
|RETURN|\ue31b|

### Keycodes
Special Keycodes also exist
|Label Name | Value|
|--|--|
|BACKSPACE|\u0008|
|SPACE| |
|RETURN| \n |


## Disclaimers

This project is based on [UltraLeap's XR Keyboard](https://github.com/ultraleap/XR-Keyboard) which is licensed under [Apache 2.0](https://github.com/ultraleap/XR-Keyboard/blob/main/LICENSE.txt)
This project is licensed under the [Magic Leap Developer License](LICENSE)

