# Unity Composition Tools

**A collection of composition and cinematography guides for Unity.**

Unity Composition Tools is an Editor extension designed to help game developers, technical artists and visual artists improve framing, composition and visual consistency directly inside the Unity Game View.

The package provides customizable composition overlays, allowing you to evaluate camera framing and cinematic layouts without modifying your original scene or camera output.

<p align="left">
  <img src="https://img.shields.io/badge/Unity-6.0%2B-black?logo=unity" alt="Unity 6.0+"/>
  <img src="https://img.shields.io/badge/Package%20Manager-UPM-blue" alt="UPM"/>
  <img src="https://img.shields.io/badge/License-MIT-green" alt="MIT License"/>
  <img src="https://img.shields.io/badge/Status-Preview-orange" alt="Preview"/>
</p>

---

## Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Requirements](#-requirements)
- [Installation](#-installation)
- [Getting Started](#-getting-started)
- [Composition Guides](#-composition-guides)
- [Customization](#-customization)
- [Debugging](#-debugging)
- [Compatibility](#-compatibility)
- [Contributing](#-contributing)
- [License](#-license)

---

## Overview

Composition Tools provides a centralized workspace for creating and managing visual guides inside the Unity Editor.

Instead of manually creating guide objects, modifying camera settings or importing external assets, you can enable composition overlays directly from a dedicated Editor Window.

**Designed for:**
- Technical Artists
- Game Developers
- Environment Artists
- Level Designers
- Cinematic Artists
- UI and Visual Designers

## Features

| Feature | Description |
|:---|:---|
| **Rule of Thirds** | A customizable 3×3 composition grid. |
| **Letterbox** | Cinematic framing with adjustable aspect ratios. |
| **Action Safe** | Rectangular safe-area guide, enabled by default at 95%. |
| **Custom Colors** | Customize the appearance of composition guides. |
| **Line Thickness** | Adjust overlay line thickness. |
| **Camera Selection** | Choose the camera used to display the composition guides. |
| **Debug System** | Diagnostic messages for camera and rendering callbacks. |

---

## Requirements

- Unity 6.0 or newer.
- Unity Package Manager (UPM).
- Git client installed and available in the system PATH.

> **Note:** Git 2.14.0 or newer is required for Git-based Package Manager installation.

---

## Installation

### Option 1 - Unity Package Manager (Recommended)

1. Open your Unity project.
2. Navigate to **Window → Package Management → Package Manager**.
3. Click the **+** button in the Package Manager window.
4. Select **Install package from git URL**.
5. Paste the repository URL:

```text
https://github.com/RafaelColpani/Unity-Composition-Tools.git
```

6. Click **Install**.

### Option 2 - Package Manifest

You can also install the package directly through your project's `Packages/manifest.json` file.

Add the following dependency:

```json
{
  "dependencies": {
    "rafaelcolpani.composition-tools": "https://github.com/RafaelColpani/Unity-Composition-Tools.git#v0.1.0"
  }
}
```

## Getting Started

Once the package is installed:

1. Open your Unity project.
2. Navigate to **Tools → Composition → Rule of Thirds**.
3. Select the camera that renders your Game View.
4. Click **Enable Composition Overlay**.
5. Enable or disable individual composition guides.

The Composition Tools window provides centralized controls for configuring your visual guides.

---

## Composition Guides

### 1. Rule of Thirds

A classic composition guide that divides the frame into nine equal sections using two horizontal and two vertical lines.

**Available controls:**
- Enable / Disable.
- Custom line color.
- Adjustable line thickness.

Useful for framing subjects, balancing visual elements and positioning points of interest.

### 2. Letterbox

Create cinematic framing by defining a target aspect ratio.

**Supported presets:**

| Preset | Aspect Ratio |
|:---|:---:|
| Widescreen | 16:9 |
| Cinema | 1.85:1 |
| Cinemascope | 2.35:1 |
| Anamorphic | 2.39:1 |
| Custom | 1.0–3.5 |

**Available controls:**
- Enable / Disable.
- Custom aspect ratio.
- Custom bar color.

The Rule of Thirds and Action Safe guides automatically adapt to the active Letterbox area.

### 3. Action Safe

A rectangular composition guide used to define a safe area within the frame.

**Default configuration:**

| Setting | Default |
|:---|:---|
| Safe Area | 95% |
| Guide Color | Red |
| Shape | Rectangular |
| Position | Centered |

**Available controls:**
- Enable / Disable.
- Adjustable safe-area percentage.
- Custom guide color.

Useful for keeping important visual elements within a defined area of the composition.

---

## Customization

Each composition guide can be controlled independently.

| Guide | Independent Toggle | Customization |
|:---|:---:|:---|
| Rule of Thirds | ✅ | Color and thickness |
| Letterbox | ✅ | Aspect ratio and bar color |
| Action Safe | ✅ | Percentage and guide color |

This allows you to combine different guides according to your visual workflow.

---

## Debugging

Composition Tools includes a diagnostic system to help identify camera and rendering issues.

Enable **Debug Logs** inside the Editor Window to display diagnostic messages in the Unity Console.

Example:

```text
[RuleOfThirds] Overlay enabled.
[RuleOfThirds] Render callback received.
[RuleOfThirds] Material created.
```

Use these messages to investigate camera selection, material creation and rendering callback execution.

---

## Compatibility

| Unity Version | Render Pipeline | Status |
|:---|:---|:---|
| Unity 6.0+ | URP | ✅ Tested |
| Unity 6.0+ | Built-in | ⚠️ Not fully validated |
| Unity 6.0+ | HDRP | ❌ Not supported |

**URP:** Successfully tested in the Unity Editor Game View. The package also compiles in a URP project.

## Known Limitations

- Composition overlays are Editor-only and are not rendered in standalone player builds.
- Letterbox simulates cinematic framing; it does not change the camera's actual output resolution or perform a physical render crop.
- The selected Game Camera must be the camera actually rendering the Game View.

---

## Contributing

Contributions, suggestions and bug reports are welcome.

To contribute:

1. Fork the repository.
2. Create a feature branch.
3. Make your changes.
4. Submit a Pull Request with a description of your implementation.

For bug reports, include:
- Unity version.
- Render Pipeline.
- Steps to reproduce the issue.
- Relevant Console messages.

---

## License

This project is licensed under the **MIT License**.

You are free to use, modify, distribute and include this package in personal and commercial projects, subject to the terms of the license.

See the [LICENSE](LICENSE) file for details.

---
