# prpu - <ins>P</ins>hi<ins>r</ins>gos <ins>P</ins>layer, written in <ins>U</ins>nity Engine

A feature-rich Phigros player implementation built with Unity Engine and C#.

---

## 🛠️ Development Environment

| Requirement               | Specification                                                      |
| ------------------------- | ------------------------------------------------------------------ |
| **Minimum Unity Version** | [2022.3.62f1](https://unity.cn/release-notes/lts/2021/2022.3.62f1) |

---

## 🕳️ Using Chart Format

> It follows a format similar to the extended version of the official chart, and on the basis of the official chart, additional features that align with the behavior of the original version are incorporated.

<details> <summary>Chart</summary>

## Root
- `formatVersion`: int - Format version number
- `offset`: float - Offset value
- `storyBoard`: StoryBoard (nullable) - Story board
- `judgeLineList`: JudgeLine[] - List of judge lines

## StoryBoard
- `eventType`: int[] - Array of event types
- `events`: JudgeLineEvent[] - Array of events

## JudgeLineEvent
- `startTime`: int[] - Array of start times
- `endTime`: int[] - Array of end times
- `start`: float - Start value
- `end`: float - End value
- `easing`: int - Easing type
- `easingLeft`: float - Left easing value
- `easingRight`: float - Right easing value
- `bezierPoints`: float[] (nullable) - Array of bezier curve points

## TextEvent
- `startTime`: int[] - Array of start times
- `endTime`: int[] - Array of end times
- `start`: string - Start text
- `end`: string - End text
- `easing`: int - Easing type
- `easingLeft`: float - Left easing value
- `easingRight`: float - Right easing value
- `bezierPoints`: float[] (nullable) - Array of bezier curve points

## JudgeLineEventLayer
- `judgeLineMoveXEvents`: JudgeLineEvent[] - Array of judge line X-axis movement events
- `judgeLineMoveYEvents`: JudgeLineEvent[] - Array of judge line Y-axis movement events
- `judgeLineRotateEvents`: JudgeLineEvent[] - Array of judge line rotation events
- `judgeLineDisappearEvents`: JudgeLineEvent[] - Array of judge line disappearance events

## Note
- `type`: int - Note type
- `isFake`: bool - Whether it's a fake note
- `above`: bool - Whether it's above
- `startTime`: int[] - Array of start times
- `visibleTime`: int[] - Array of visible times
- `speed`: float - Speed
- `size`: float - Size
- `endTime`: int[] - Array of end times
- `positionX`: float - X-axis position
- `positionY`: float - Y-axis position
- `color`: int - Color
- `hitFXColor`: int - Hit effect color
- `judgeSize`: float - Judge size

## BpmItems
- `time`: int[] - Array of times
- `bpm`: float - BPM value

## Transform
- `judgeLineColorEvents`: JudgeLineEvent[] (nullable) - Array of judge line color events
- `judgeLineTextEvents`: TextEvent[] (nullable) - Array of judge line text events
- `fatherLineIndex`: int - Parent judge line index
- `localPositionMode`: bool - Whether in local position mode
- `localEulerAnglesMode`: bool - Whether in local Euler angles mode
- `zOrder`: int - Z-axis order
- `judgeLineTextureScaleXEvents`: JudgeLineEvent[] (nullable) - Array of judge line texture X-axis scale events
- `judgeLineTextureScaleYEvents`: JudgeLineEvent[] (nullable) - Array of judge line texture Y-axis scale events

## NoteControl
- `disappearControls`: ControlItem[] (nullable) - Array of disappearance control items
- `rotateControls`: ControlItem[] (nullable) - Array of rotation control items
- `sizeControl`: ControlItem[] (nullable) - Array of size control items
- `xPosControl`: ControlItem[] (nullable) - Array of X-axis position control items
- `yPosControl`: ControlItem[] (nullable) - Array of Y-axis position control items

## ControlItem
- `easing`: int - Easing type
- `value`: float - Control value
- `x`: float - X value

## JudgeLine
- `bpms`: BpmItems[] - Array of BPM items
- `notes`: Note[] - Array of notes
- `noteControls`: NoteControl (nullable) - Note controls
- `speedEvents`: JudgeLineEvent[] - Array of speed events
- `judgeLineEventLayers`: JudgeLineEventLayer[] - Array of judge line event layers
- `transform`: Transform - Transformation information

</details>

<details> <summary>Story Board Type</summary>

### Event Type Mapping
The `type` field corresponds to specific game properties, as defined in the class summary:

| Type Value | Target Property | Description |
|------------|-----------------|-------------|
| 0 | `mainCamera.orthographicSize` | Adjusts the 2D camera's field of view (orthographic size). |
| 1 | `mainCamera.transform.eulerAngles.z` | Rotates the main camera around the Z-axis. |
| 2 | `chartName.color.a` | Controls the alpha (transparency) of the "chart name" UI text. |
| 3 | `level.color.a` | Controls the alpha of the "level" UI text. |
| 4 | `score.color.a` | Controls the alpha of the "score" UI text. |
| 5 | `combo.color.a` | Controls the alpha of the "combo count" UI text. |
| 6 | `comboText.color.a` | Controls the alpha of the "Combo" label UI text. |
| 7 | `pauseBtn.color.a` | Controls the alpha of the pause button UI image. |
| 8 | `audioSource.volume` | Adjusts the volume of the main audio source. |
| 9 | `audioSource.pitch` | Adjusts the pitch (speed/tone) of the main audio source. |

</details>

---

## 📊 Supported Chart Formats

| Format    | Supported Versions   | Compatibility Status |
| --------- | -------------------- | -------------------- |
| Phigros   | 100 - 250 (fv1, fv3) | ✅ Full Support      |
| RePhiedit | 100 - 170            | ⚠️ Partial Support ¹ |

> ¹ Limitations include: incomplete support for Attract UI and GIF content, and restricted judgment line texture references via chart routes for security and stability.

---

## ✨ Core Features

- **Advanced Storyboard System**  
  Implement custom game control behaviors with comprehensive Level Mods functionality.

- **Addressable Technology Integration**  
  Enables efficient resource management and on-demand hot loading capabilities.

- **Unity Recorder**  
  Implement the chart rendering function using the official built-in recorder.

---

## 📦 Used Resources

### Typography

- **Source Han Sans** by Adobe  
  [Repository](https://github.com/adobe-fonts/source-han-sans)
- **Saira** by Omnibus Type  
  [Repository](https://github.com/Omnibus-Type/Saira)

### Visual Assets

- **Phigros Skin** (Pigeon Games)

---

## 📜 License Information

This project is distributed under the [GNU General Public License v3.0](LICENSE).

> ⚠️ Important Notice: While highly unlikely, this project may incorporate third-party plugins("Assets/Plugins" or "Assets/xxx 's Assets") in the future that utilize different licensing terms. Such plugins are not covered by the project's main license and should not be copied, modified, or integrated into your projects (including forks) without explicit permission from their respective copyright holders. The project maintainers assume no liability for any issues arising from non-compliance with these separate licensing terms.

---

## 🪼 Acknowledgements

### References & Inspirations

- **Lchzh Docs**  
  [Link](https://docs.lchzh.net/learning/phigros/)  
  Referenced documentation structure and content.

- **Phira** by TeamFlos  
  [Repository](https://github.com/TeamFlos/phira/releases)  
  Adopted its "aggressive optimization" approach for note generation, which significantly improved performance.

- **Phiplayer**  
  [Repository](https://gitee.com/z-1029/phiplayer)  
  Borrowed judgment line update method.

---

_Built with Unity Engine | C#_
