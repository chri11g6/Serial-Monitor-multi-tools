byte tal = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
    Serial.println(tal);
    tal++;

    if((tal % 10) == 0){
        Serial.println("${88}");
    }
    
    delay(500);
}
