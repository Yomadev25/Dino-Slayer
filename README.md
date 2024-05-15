# Dino Slayer
เป็นเกม Local Multiplayer ที่ให้ผู้เล่นนำโทรศัพท์มาสแกน QR Code บนจอที่แสดงตัวเกม แล้วจะแสดง Joystick ออกมาให้ผู้เล่นได้ควบคุมตัวละคร
![Untitled](https://github.com/Yomadev25/Dino-Slayer/assets/61727972/653d0a87-c6bf-407b-9487-140aed12437d)

🦖 ทดลองเล่นบน WebGL ได้ที่ https://yomadev25.github.io/Virtual-Joystick-Game/game
🦖 สามารถดาวน์โหลดตัวเกมหลักได้ที่ https://drive.google.com/file/d/1A86V9eaP_llnLQf_DN6e36r71Dt7AmPv/view?usp=sharing

# Tools
- Unity Engine
- Photon (PUN2)
# Scene

ในโปรเจคจำเป็นที่จะต้อง Build แยกออกมา 2 scene คือ **Game** สำหรับ Window Build และ **Player** สำหรับ WebGL Build

## Game scene

Scene สำหรับเป็นศูนย์กลางแสดงตัวเกมบนหน้าจอ และคอยเช็ค State ต่างๆในเกม สามารถ Build ออกมาเป็น Window Application ได้ตามปกติ

## Player scene

Scene สำหรับเป็น Joystick ให้กับผู้เล่นได้ควบคุมตัวละครใน **Game scene** โดยจะส่งค่า Axis ของ Joystick ผ่าน Method
 ```csharp
PhotonNetwork.RaiseEvent();
```

แล้วให้ GameManager.cs ใน **Game scene** รับข้อมูล Event เหล่านั้นเพื่อควบคุมตัวละคร

อ่านข้อมูลเพิ่มเติมเกี่ยวกับ RaiseEvent() ของ Photon ได้ที่ https://doc.photonengine.com/pun/current/gameplay/rpcsandraiseevent


## Credit
- Dino Character Sprites [@ScissorMarks](https://twitter.com/ScissorMarks) 
- Environment Sprites https://analogstudios.itch.io/fantasy
