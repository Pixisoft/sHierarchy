[![License: Private](https://img.shields.io/badge/License-Private-green.svg)](https://github.com/Pixisoft/sHierarchy/blob/master/COPYING)
[![Unity Engine](https://img.shields.io/badge/unity-2021.2.0f1-black.svg?style=flat&logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)

# sHierarchy
> Minimalistic good looking hierarchy

[![License](https://github.com/Pixisoft/sHierarchy/actions/workflows/license.yml/badge.svg)](https://github.com/Pixisoft/sHierarchy/actions/workflows/license.yml)
[![Source ⚙️](https://github.com/Pixisoft/sHierarchy/actions/workflows/source_build.yml/badge.svg)](https://github.com/Pixisoft/sHierarchy/actions/workflows/source_build.yml)
[![Source 📦](https://github.com/Pixisoft/sHierarchy/actions/workflows/source_package.yml/badge.svg)](https://github.com/Pixisoft/sHierarchy/actions/workflows/source_package.yml)
[![Publish ⚙️](https://github.com/Pixisoft/sHierarchy/actions/workflows/publish_build.yml/badge.svg)](https://github.com/Pixisoft/sHierarchy/actions/workflows/publish_build.yml)
[![Publish 📦](https://github.com/Pixisoft/sHierarchy/actions/workflows/publish_package.yml/badge.svg)](https://github.com/Pixisoft/sHierarchy/actions/workflows/publish_package.yml)

<p align="center">
  <img src="./etc/showcase.png" />
</p>

## 📁 Project Structures

* `Source` - Project contains plugin's source code.
* `Compile` - Project compiles source to DLL, it will link `_references` DLLs.
* `_references` - Unity DLL to compile project source to DLL.
* `Publish` - Project that contains package DLL and ready to publish to [Asset Store Publisher](https://publisher.assetstore.unity3d.com/info.html?_gl=1*1fwg1ij*_ga*MTg0NjU4MTc4NC4xNjAwMzQ5NzM3*_ga_1S78EFL1W5*MTYyNDI3MzU4Ni40Ni4wLjE2MjQyNzM1ODYuNjA.&_ga=2.77544981.1416380940.1624186429-1846581784.1600349737) portal.
* `Test` - Project to test if everything work well to new empty Unity project.

## 🏆 Features

* Show enable/disable state of each components
* Show log/warning/error icon
* Clean, no files are generated
* Clean, no other resource files
* Lightweight, no other dependencies
* No redundant workflow
* Work in Prefab Mode

## License

Copyright (c) Pixisoft. All rights reserved.

pixisoft.tw@gmail.com
