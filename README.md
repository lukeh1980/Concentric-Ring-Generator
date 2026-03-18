# Concentric-Ring-Generator
This is a simple framework written in C# for the Unity Game Engine. The CRG base classes can be used to generate concentric rings of objects with full control over placement, rotation and randomization. I extended the base classes to build the "Stone Circle Generator" utility that was once available on the Unity Asset Store. I have since slightly modified and republished the utility as a free asset that is again available on the Unity Asset Store. Watch the video for a demonstration of the features here: https://www.youtube.com/watch?v=M8XP35rHC_s

All features are demonstrated execept for the runtime API that I've also added. The original utility was only designed to work in the Unity Editor, allowing you to generate complex structures for placement within your game. The runtime API allows you to generate these in-game with almost all features implemented.

Materials can be easily converted to HDRP materials using the built in wizard in unity. I have materials for the Built-in Render Pipeline for download as well but it will require manual assignment as there isn't an easy way to covert from URP or HDRP to BRP. 

Full source code is available, easily extendable. Simply drag and drop the "DigitalHorde" folder in your Unity project's Assets folder to use. This version has been tested on Unity versions 2021.3 - 6000.3
