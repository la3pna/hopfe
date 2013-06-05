int col =          5;   // col. PWM output
int base =         3;   // Base voltage output
int j  =           0;   // value for col PWM
int k =            0;   // value for base PWM
int slope =        4;   // Slope of the triangular wave on col. 
int stepno =       5;   // number of steps
int pol =          6;   // Port for change of base polarity
int smps =         13;  // Selection of SMPS
int neg =          12;  // Selection of polarity
int ResA =         11;  // Base resistor 1 switch  
int ResB =         10;  // Base resistor 2 switch  
int ResC =         9;   // Base resistor 3 switch  
int ResD =         8;   // Base resistor 4 switch  
int                inData;  // RS232 data 

void               setup() 
{ 
  Serial.begin(      9600);  // initialize serial communication:
  pinMode( col, OUTPUT);
  pinMode( base,  OUTPUT);
  pinMode( pol,  OUTPUT);
  pinMode( smps, OUTPUT);
  pinMode( neg, OUTPUT);
  pinMode( ResA, OUTPUT);
  pinMode( ResB, OUTPUT);
  pinMode( ResC, OUTPUT);
  pinMode( ResD, OUTPUT);
  digitalWrite ( pol,LOW);
  digitalWrite ( smps,LOW);
  digitalWrite ( neg,LOW);
  digitalWrite ( ResA,LOW);
  digitalWrite ( ResB,LOW);
  digitalWrite ( ResC,LOW);
  digitalWrite ( ResD,LOW);
          
}// setup 

void loop() { 
  if (Serial.available() > 0)   // see if incoming serial data:
  { 
    inData = Serial.read();  // read oldest byte in serial buffer:
  } 
  
   if (inData == 'I')  // if I (ASCII 73), printoutput
  {
        digitalWrite(pol, HIGH );        // Turn on negative base supply
        inData = 0;
          Serial.println("HIGH");
  }
     if (inData == 'J') {                                          
        digitalWrite(pol,LOW);         // Turn on positive base supply
        inData = 0;
          Serial.println("LOW");
  
  }
  
  if (inData == 'H')  // if H (ASCII 72) do measurment
  { 
    delay(10);
    j =                j+slope;
    analogWrite(       col, j);                   // will be PWM 488 Hz
    analogWrite(       base, k);                  // will be PWM 488 Hz
    Serial.print(      analogRead(0));            // read current  at A0
      Serial.print(      " ");                                    
    Serial.println(    analogRead(1));            // read col voltage at A1
      delay(             10);                     // to stabilize adc:
    if                 (j > 251) slope = -4 ; 
    if                 (j < 1)   
    { 
      slope =            4 ;
      k =                k + int(255/stepno);
    } 
    if                 (k > 255 )   k = 0, inData = 0;
  
  } 
  
    if (inData == 'K')  // if H (ASCII 75), Select NPN setup
  {
    
    
        digitalWrite(pol, HIGH );        
        inData = 0;
          Serial.println("HIGH");
      }
      
      if (inData == 'L'){    // Select PNP setup                                      
        digitalWrite(pol,LOW);
        inData = 0;
  }

    if (inData == 'M')  // if H (ASCII 72), enable SMPS
  {
    
      
        digitalWrite(smps, HIGH );        
        inData = 0;
          Serial.println("HIGH");
  }
      
 if (inData == 'N'){          // if N (ASCII 78) de-enable SMPS
        digitalWrite(smps,LOW);
        inData = 0;
  }
  

 if (inData == 'Y')  // if H (ASCII 89), printoutput
  {
    
     if ( digitalRead(smps) == LOW  ) { // Button not pushed
        stepno = stepno + 1;        
        inData = 0;
          Serial.println(stepno);
     }
   }
  if (inData == 'Z')  // if z (ASCII 90), increase steps
  {
    
      stepno = stepno - 1;
           // Turn off the LED
        inData = 0;
        if (stepno <= 0 ){
          stepno = 1;
        }
          Serial.println(stepno);
   
  }

  if (inData == 'A')  // Select resistor 1
  {
    
      if ( digitalRead(neg) == LOW  ) { // Button not pushed
        digitalWrite(ResA, HIGH );        // Turn off the LED
        inData = 0;
          Serial.println("HIGH");
     } else {                                          // Button is pushed
        digitalWrite(ResA,LOW);
        inData = 0;
   
  }
  }
  if (inData == 'B')  // select resistor 2
  {
    
   if ( digitalRead(ResB) == LOW  ) { // Button not pushed
        digitalWrite(ResB, HIGH );        // Turn off the LED
        inData = 0;
          Serial.println("HIGH");
     } else {                                          // Button is pushed
        digitalWrite(ResB,LOW);
        inData = 0;
   
  }
  }
  if (inData == 'C')  // select resistor 3
  {
    
    if ( digitalRead(ResC) == LOW  ) { // Button not pushed
        digitalWrite(ResC, HIGH );        // Turn off the LED
        inData = 0;
          Serial.println("HIGH");
     } else {                                          // Button is pushed
        digitalWrite(ResC,LOW);
        inData = 0;
   
  }
  }
  if (inData == 'D')  // select resistor 4
  {
    
     if ( digitalRead(ResD) == LOW  ) { // Button not pushed
        digitalWrite(ResD, HIGH );        // Turn off the LED
        inData = 0;
          Serial.println("HIGH");
     } else {                                          // Button is pushed
        digitalWrite(ResD,LOW);
        inData = 0;
     }
  }
  
} //                 loop()



