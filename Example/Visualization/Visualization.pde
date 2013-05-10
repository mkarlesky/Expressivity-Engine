import peasy.*;
import processing.opengl.*;
import processing.net.*; 

String inString;
byte newline = 10;

Client clientA;
Client clientB;
PeasyCam camera;
  
float SPHERE_SIZE     = 50.0;
float SPHERE_SPACING  = 250.0;

PVector expressivityA;
PVector expressivityB;

/*
 * A scene of two spheres whose size and position is controlled by two expressivity vectors
 * transmitted over the network from two independent Kinect setups
 * (two people facing each other).
 */

void setup()  { 
  size(1024, 512, OPENGL); 
  noStroke(); 
  colorMode(RGB, 1);

  // Movable viewing camera for the scene
  camera = new PeasyCam(this, 500);
  camera.setMinimumDistance(100);
  camera.setMaximumDistance(1000);
  camera.lookAt(0, 0, 0);
  
  expressivityA = new PVector(0, 0, 0);
  expressivityB = new PVector(0, 0, 0);
  
  // Network configuration
  clientA = new Client(this, "127.0.0.1", 5204);
  clientB = new Client(this, "169.254.127.108", 5204);
}
 
void draw()  { 
  background(0.3);
  
  // Read and parse vector components from string sent over network
  if (clientA.available() > 0) { 
    inString = clientA.readStringUntil(newline);
    try {
      String[] vals = splitTokens(inString, "|");
      expressivityA.x = float(vals[0]);
      expressivityA.y = float(vals[1]);
      expressivityA.z = float(vals[2]);
      expressivityA.mult(5000);
    } catch(Exception e) {
    }
  }

  // Read and parse vector components from string sent over network
  if (clientB.available() > 0) { 
    inString = clientB.readStringUntil(newline);
    try {
      String[] vals = splitTokens(inString, "|");
      expressivityB.x = float(vals[0]);
      expressivityB.y = float(vals[1]);
      expressivityB.z = float(vals[2]);
      expressivityB.mult(5000);
    } catch(Exception e) {
    }
  }

  /*
   * Construct the scene
   */
    
  lights();

  // Left Marker
  pushMatrix();  

  translate(-SPHERE_SPACING, 0, 0);

  box(10);
  
  popMatrix(); 

  // Right Marker
  pushMatrix();  

  translate(SPHERE_SPACING, 0, 0);

  box(10);
  
  popMatrix(); 

  // Left Sphere
  pushMatrix();  

  translate(-SPHERE_SPACING + expressivityA.z, -expressivityA.y, expressivityA.x);
  sphere(expressivityA.mag() * 0.9);
  
  popMatrix(); 

  // Right Sphere
  pushMatrix(); 

  translate(SPHERE_SPACING - expressivityB.z, -expressivityB.y, expressivityB.x);
  sphere(expressivityB.mag() * 0.9);
  
  popMatrix(); 
}

