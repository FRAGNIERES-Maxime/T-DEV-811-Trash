#include "LedController.hpp"
#include "SR04.h"

#define TRIG_PIN 9
#define ECHO_PIN 8

#define pinTilt 52
#define pinIR A0
#define pinReset 40

#define pinLed1 39
#define pinLed2 43
#define pinLed3 47

#define DIN 27 // pin DIN Matrice
#define CS 26 // pin CS Matrice
#define CLK 25 // pin CLK Matrice
#define Segments 1 // Nb Matrices
#define delayTime 200 // Delay between Frames

 // instance du controlleur de matrice
LedController lc = LedController(DIN,CLK,CS,Segments);

ByteBlock pictoOn = {
  B11111111,
  B11111111,
  B11111111,
  B11111111,
  B11111111,
  B11111111,
  B11111111,
  B11111111
};

SR04 sr04 = SR04(ECHO_PIN,TRIG_PIN);
long distance;

bool allChanged = false;

void setup() {
  // put your setup code here, to run once:
  pinMode(pinTilt, INPUT);
  pinMode(pinIR, INPUT);
  pinMode(pinReset, INPUT);
  pinMode(pinLed1, OUTPUT);
  pinMode(pinLed2, OUTPUT);
  pinMode(pinLed3, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  
  int tiltState = digitalRead(pinTilt);
  int IRState;
  int resetState = digitalRead(pinReset);
  int ultrasonState;
  
  distance=sr04.Distance();
   int valueLRD = analogRead(pinIR);
   if(valueLRD < 200)
  {
    IRState = 1;
  }else
  {
    IRState=0;
  }
  if(distance < 5)
  {
    ultrasonState = 1;
  }else
  {
    ultrasonState = 0;
  }
  Serial.print(tiltState);
  Serial.print("/");
  Serial.print(valueLRD);
  Serial.print("/");
  Serial.print(distance);
  Serial.print("/");
  Serial.print(resetState);
  Serial.print("/");
  Serial.println(allChanged);
  if((!tiltState) && IRState && ultrasonState)
  {
    allChanged = true;
  }
  if(allChanged)
  {
    lc.displayOnSegment(0,pictoOn);
  }else
  {
    lc.clearMatrix();
  }
  if(resetState)
  {
    allChanged = false;
  }
  digitalWrite(pinLed1, !tiltState);
  digitalWrite(pinLed2, ultrasonState);
  digitalWrite(pinLed3, IRState);
  
  delay(10);

}
