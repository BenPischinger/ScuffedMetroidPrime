# Scuffed Metroid Prime
 
 This is a project to re-create some gameplay aspects of Metroid Prime in Unity.
 
 At the moment if features a first-person mode and a third-person morphball mode.
 
 
  ## First-Person Mode Features
<p align="center">
<img src="https://i.imgur.com/zZkSNYv.png" width="1385">
</p>
 
 - Movement is achieved through a simple character controller and the 'WASD' keys
 - Arm Cannon can be shot by pressing the left mouse button and charged by holding down the left mouse button
 - Rockets can be shot by pressing the right mouse button
 - Using shift while walking executes a quick dash in the walking direciton
 - Space can be pressed twice to execute a double jump
 - Swap into third-person mode by pressing 'Q'
 
 - Dash cooldown and rocket ammo can be tracked on the HUD
 
 - The helmet and arm cannon feature a swivel by lerping when the camera is moved around
 
 ## Third-Person (Morphball) Mode Features
 
 <p align="center">
<img src="https://i.imgur.com/OriCeaP.png" width="1385">
</p>

- The morphball is a rigidbody that moves by applying force in the camera direction when 'WASD' is pressed
- Pressing the left mouse button drops bombs, which are also based on a rigidbody
- The bombs apply force in a radius upon explosion and can be used to blast the morphball or other bombs into the air
- There's also some assets in the project's main scene that can be destroyed with the morphball bombs

## Assets
- <a href="https://sketchfab.com/3d-models/samus-25cdda3ea14f42ce86b4dd089bd417ce">Arm Cannon</a>

- <a href="https://www.instagram.com/vitorm.95/">HUD Elements</a>

- <a href="https://sketchfab.com/3d-models/metroid-morphball-d2cdb2cf0bff4ffb924dd9d8a4c5746e">Morphball</a>

- <a href="https://sketchfab.com/3d-models/gunship-metroid-f098be8ed060432b86bc36a7dd64887b">Gunship</a>




