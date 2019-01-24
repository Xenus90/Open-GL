using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;        
        uint[] texture = new uint [1];
        public int movingPart = 0;
        public bool enableReflection = true;
        public bool enableRotate = false;
        public bool enableLightSource = false;
        public bool enableDefaultCoordSystem = false;
        public bool enableLookAtValue = false;
        public int coordSysMoveDirection = 0;
        public float viewAngle = 0.0f;
        public float[] indexFingerAngle = { 0, 0, 0 };
        public float[] middleFingerAngle = { 0, 0, 0 };
        public float[] ringFingerAngle = { 0, 0, 0 };
        public float[] pinkyFingerAngle = { 0, 0, 0 };
        public float[] thumbFingerAngle = { 0, 0 };
        public float[] floorAngle = { 0, 0, 0 };
        public float[] lightPosition = { 0.0f, 0.0f, 0.0f, 1.0f };
        public float[] coordSysNewPosition = new float[3];
        public float[] coordSysRotationNewPosition = new float[3];
        public float[] LookAtNumberValue = { 0, 1, 15, 0, 0, 0, 0, 1, 0 };
        public double[] AccumulatedTraslations = new double[16];

        GLUquadric objFinger;
        GLUquadric objShpere;
             
        uint handList, handBaseList;
        uint indexPhalanxList1, indexPhalanxList2, indexPhalanxList3, indexFingerStart;
        uint middlePhalanxList1, middlePhalanxList2, middlePhalanxList3, middleFingerStart; 
        uint ringPhalanxList1, ringPhalanxList2, ringPhalanxList3, ringFingerStart;       
        uint pinkyPhalanxList1, pinkyPhalanxList2, pinkyPhalanxList3, pinkyFingerStart;    
        uint thumbPhalanxList1, thumbPhalanxList2, thumbFingerStart;
        uint floorMainList, floorList, floorStartPosition;

        uint m_uint_HWND = 0;
        uint m_uint_DC = 0;
        uint m_uint_RC = 0;

        float[] planeCoeff = { 1, 1, 1, 1 };
        float[,] ground = new float[3, 3];
        float[] cubeXform = new float[16];
        const int x = 0;
        const int y = 1;
        const int z = 2;
        
        public cOGL(Control pb)
        {
            p = pb;
            Width = p.Width;
            Height = p.Height;
            objFinger = GLU.gluNewQuadric();
            objShpere = GLU.gluNewQuadric();
                      
            InitializeGL();
            PrepareLists();

            ground[0, 0] = 1;
            ground[0, 1] = 1;
            ground[0, 2] = -0.5f;

            ground[1, 0] = 0;
            ground[1, 1] = 1;
            ground[1, 2] = -0.5f;

            ground[2, 0] = 1;
            ground[2, 1] = 0;
            ground[2, 2] = -0.5f;
        }

        void InitTexture(string imageBMPfile)
        {
            GL.glEnable(GL.GL_TEXTURE_2D);

            Bitmap image = new Bitmap(imageBMPfile);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            System.Drawing.Imaging.BitmapData bitmapdata;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.glGenTextures(1, texture);
            GL.glBindTexture(GL.GL_TEXTURE_2D, texture[0]);
            GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height, 0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
            GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
            GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

            image.UnlockBits(bitmapdata);
            image.Dispose();
        }

        void ReduceToUnit(float[] vector)
        {
            float length;

            length = (float)Math.Sqrt((vector[0] * vector[0]) + (vector[1] * vector[1]) + (vector[2] * vector[2]));

            if (length == 0.0f)
                length = 1.0f;

            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            ReduceToUnit(outp);
        }

        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            calcNormal(points, planeCoeff);

            planeCoeff[3] = -((planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) + (planeCoeff[2] * points[2, 2]));

            dot = planeCoeff[0] * lightPosition[0] + planeCoeff[1] * lightPosition[1] + planeCoeff[2] * lightPosition[2] + planeCoeff[3];

            cubeXform[0] = dot - lightPosition[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - lightPosition[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - lightPosition[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - lightPosition[0] * planeCoeff[3];

            cubeXform[1] = 0.0f - lightPosition[1] * planeCoeff[0];
            cubeXform[5] = dot - lightPosition[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - lightPosition[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - lightPosition[1] * planeCoeff[3];

            cubeXform[2] = 0.0f - lightPosition[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - lightPosition[2] * planeCoeff[1];
            cubeXform[10] = dot - lightPosition[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - lightPosition[2] * planeCoeff[3];

            cubeXform[3] = 0.0f - lightPosition[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - lightPosition[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - lightPosition[3] * planeCoeff[2];
            cubeXform[15] = dot - lightPosition[3] * planeCoeff[3];
        }

        void DrawFigures()
        {
            GL.glPushMatrix();
            GL.glEnable(GL.GL_COLOR_MATERIAL);

            DrawLightSource();

            GL.glCallList(handList);
            GL.glEnd();
            GL.glPopMatrix();

            GL.glPushMatrix();

            if (enableLightSource)
                MakeShadowMatrix(ground);

            GL.glMultMatrixf(cubeXform);
            GL.glPopMatrix();
        }

        public void PrepareLists()
        {
            handList = GL.glGenLists(21);
            handBaseList = handList + 1;
            indexFingerStart = handList + 2;
            middleFingerStart = handList + 3;
            ringFingerStart = handList + 4;
            pinkyFingerStart = handList + 5;
            thumbFingerStart = handList + 6;
            indexPhalanxList1 = handList + 7;
            indexPhalanxList2 = handList + 8;
            indexPhalanxList3 = handList + 9;
            middlePhalanxList1 = handList + 10;
            middlePhalanxList2 = handList + 11;
            middlePhalanxList3 = handList + 12;
            ringPhalanxList1 = handList + 13;
            ringPhalanxList2 = handList + 14;
            ringPhalanxList3 = handList + 15;
            pinkyPhalanxList1 = handList + 16;
            pinkyPhalanxList2 = handList + 17;
            pinkyPhalanxList3 = handList + 18;
            thumbPhalanxList1 = handList + 19;
            thumbPhalanxList2 = handList + 20;

            floorMainList = GL.glGenLists(3);
            floorStartPosition = floorMainList + 1;
            floorList = floorMainList + 2;

            //Floor Start Position
            GL.glPushMatrix();
            GL.glNewList(floorStartPosition, GL.GL_COMPILE);
            GL.glTranslated(0, 0, 0);
            GL.glEndList();
            GL.glPopMatrix();

            //Floor
            GL.glPushMatrix();
            GL.glNewList(floorList, GL.GL_COMPILE);
            DrawFloor();
            GL.glEndList();
            GL.glPopMatrix();

            //Hand Base
            GL.glPushMatrix();
            GL.glNewList(handBaseList, GL.GL_COMPILE);
            DrawHand();
            GL.glEndList();
            GL.glPopMatrix();

            //Index Finger Start Position
            GL.glPushMatrix();
            GL.glNewList(indexFingerStart, GL.GL_COMPILE);
            GL.glTranslated(2.5, 2.3, 0);
            GL.glEndList();
            GL.glPopMatrix();           

            //Middle Finger Start Position
            GL.glPushMatrix();
            GL.glNewList(middleFingerStart, GL.GL_COMPILE);
            GL.glTranslated(0.9, 2.3, 0);
            GL.glEndList();
            GL.glPopMatrix();

            //Ring Finger Start Position
            GL.glPushMatrix();
            GL.glNewList(ringFingerStart, GL.GL_COMPILE);
            GL.glTranslated(-0.7, 2.3, 0);
            GL.glEndList();
            GL.glPopMatrix();

            //Pinky Finger Start Position
            GL.glPushMatrix();
            GL.glNewList(pinkyFingerStart, GL.GL_COMPILE);
            GL.glTranslated(-2.5, 2.3, 0);
            GL.glEndList();
            GL.glPopMatrix();

            //Thumb Finger Start Position
            GL.glPushMatrix();
            GL.glNewList(thumbFingerStart, GL.GL_COMPILE);
            GL.glTranslated(3.35, 0.6, 0);
            GL.glEndList();
            GL.glPopMatrix();

            //[1/3] - Index Finger
            GL.glPushMatrix(); 
            GL.glNewList(indexPhalanxList1, GL.GL_COMPILE);
            DrawSingleFinger(1, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[2/3]
            GL.glPushMatrix(); 
            GL.glNewList(indexPhalanxList2, GL.GL_COMPILE);
            DrawSingleFinger(2, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[3/3]
            GL.glPushMatrix(); 
            GL.glNewList(indexPhalanxList3, GL.GL_COMPILE);
            DrawSingleFinger(3, 2.5);
            GL.glEndList();
            GL.glPopMatrix();

            //[1/3] - Middle Finger
            GL.glPushMatrix();
            GL.glNewList(middlePhalanxList1, GL.GL_COMPILE);
            DrawSingleFinger(1, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[2/3]
            GL.glPushMatrix();
            GL.glNewList(middlePhalanxList2, GL.GL_COMPILE);
            DrawSingleFinger(2, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[3/3]
            GL.glPushMatrix();
            GL.glNewList(middlePhalanxList3, GL.GL_COMPILE);
            DrawSingleFinger(3, 0.9);
            GL.glEndList();
            GL.glPopMatrix();

            //[1/3] - Ring Finger
            GL.glPushMatrix();
            GL.glNewList(ringPhalanxList1, GL.GL_COMPILE);          
            DrawSingleFinger(1, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[2/3]
            GL.glPushMatrix();
            GL.glNewList(ringPhalanxList2, GL.GL_COMPILE);
            DrawSingleFinger(2, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[3/3]
            GL.glPushMatrix();
            GL.glNewList(ringPhalanxList3, GL.GL_COMPILE);
            DrawSingleFinger(3, -0.7);
            GL.glEndList();
            GL.glPopMatrix();

            //[1/3] - Pinky Finger
            GL.glPushMatrix();
            GL.glNewList(pinkyPhalanxList1, GL.GL_COMPILE);            
            DrawSingleFinger(1, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[2/3]
            GL.glPushMatrix();
            GL.glNewList(pinkyPhalanxList2, GL.GL_COMPILE);
            DrawSingleFinger(2, 0);
            GL.glEndList();
            GL.glPopMatrix();
            //[3/3]
            GL.glPushMatrix();
            GL.glNewList(pinkyPhalanxList3, GL.GL_COMPILE);
            DrawSingleFinger(3, -2.5);
            GL.glEndList();
            GL.glPopMatrix();

            //[1/2] - Thumb Finger
            GL.glPushMatrix();
            GL.glNewList(thumbPhalanxList1, GL.GL_COMPILE);
            DrawThumbFinger(1);
            GL.glEndList();
            GL.glPopMatrix();
            //[2/2]
            GL.glPushMatrix();
            GL.glNewList(thumbPhalanxList2, GL.GL_COMPILE);
            DrawThumbFinger(2);
            GL.glEndList();
            GL.glPopMatrix();


            CreateHandList();
            CreateFloorList();
        }

        public void CreateHandList()
        {
            GL.glPushMatrix();
            GL.glNewList(handList, GL.GL_COMPILE);

            switch (movingPart)
            {           
                case 0:
                    GL.glCallList(handBaseList);                    
                    CallFingerList(1);
                    CallFingerList(2);
                    CallFingerList(3);
                    CallFingerList(4);
                    CallFingerList(5);
                    break;                

                case 1: //Index finger move implementation
                    GL.glCallList(handBaseList);
                    CallFingerList(2);
                    CallFingerList(3);
                    CallFingerList(4);
                    CallFingerList(5);
                    GL.glCallList(indexFingerStart);
                    GL.glRotatef(indexFingerAngle[0], 1, 0, 0);
                    GL.glCallList(indexPhalanxList1);
                    GL.glRotatef(indexFingerAngle[1], 1, 0, 0);
                    GL.glCallList(indexPhalanxList2);
                    GL.glRotatef(indexFingerAngle[2], 1, 0, 0);
                    GL.glCallList(indexPhalanxList3);                    
                    break;

                case 2: //Middle finger move implementation
                    GL.glCallList(handBaseList);
                    CallFingerList(1);
                    CallFingerList(3);
                    CallFingerList(4);
                    CallFingerList(5);
                    GL.glCallList(middleFingerStart);
                    GL.glRotatef(middleFingerAngle[0], 1, 0, 0);
                    GL.glCallList(middlePhalanxList1);
                    GL.glRotatef(middleFingerAngle[1], 1, 0, 0);
                    GL.glCallList(middlePhalanxList2);
                    GL.glRotatef(middleFingerAngle[2], 1, 0, 0);
                    GL.glCallList(middlePhalanxList3);
                    break;

                case 3: //Ring finger move implementation
                    GL.glCallList(handBaseList);
                    CallFingerList(1);
                    CallFingerList(2);
                    CallFingerList(4);
                    CallFingerList(5);
                    GL.glCallList(ringFingerStart);
                    GL.glRotatef(ringFingerAngle[0], 1, 0, 0);
                    GL.glCallList(ringPhalanxList1);
                    GL.glRotatef(ringFingerAngle[1], 1, 0, 0);
                    GL.glCallList(ringPhalanxList2);
                    GL.glRotatef(ringFingerAngle[2], 1, 0, 0);
                    GL.glCallList(ringPhalanxList3);
                    break;

                case 4: //Pinky finger move implementation
                    GL.glCallList(handBaseList);
                    CallFingerList(1);
                    CallFingerList(2);
                    CallFingerList(3);
                    CallFingerList(5);
                    GL.glCallList(pinkyFingerStart);
                    GL.glRotatef(pinkyFingerAngle[0], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList1);
                    GL.glRotatef(pinkyFingerAngle[1], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList2);
                    GL.glRotatef(pinkyFingerAngle[2], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList3);
                    break;

                case 5: //Thumb finger move implementation
                    GL.glCallList(handBaseList);
                    CallFingerList(1);
                    CallFingerList(2);
                    CallFingerList(3);
                    CallFingerList(4);
                    GL.glCallList(thumbFingerStart);
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glRotatef(thumbFingerAngle[0], 1, 0, 0);
                    GL.glRotated(45, 0, 0, 1);
                    GL.glCallList(thumbPhalanxList1);
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glRotatef(thumbFingerAngle[1], 1, 0, 0);
                    GL.glRotated(45, 0, 0, 1);
                    GL.glCallList(thumbPhalanxList2);
                    break;
            }

            GL.glEndList();
            GL.glPopMatrix();
        }

        public void CreateFloorList()
        {
            GL.glPushMatrix();
            GL.glNewList(floorMainList, GL.GL_COMPILE);

            GL.glCallList(floorStartPosition);
            GL.glCallList(floorList);

            GL.glEndList();
            GL.glPopMatrix();
        }

        public void Draw()
        {
            float delta = 0;
            double[] ModelViewMatrixBeforeTransforms = new double[16];
            double[] CurrentTraslation = new double[16];

            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
            GL.glLoadIdentity();

            if (enableLookAtValue)
            {
                GLU.gluLookAt(LookAtNumberValue[0], LookAtNumberValue[1], LookAtNumberValue[2],
                              LookAtNumberValue[3], LookAtNumberValue[4], LookAtNumberValue[5],
                              LookAtNumberValue[6], LookAtNumberValue[7], LookAtNumberValue[8]);
            }          

            GL.glTranslatef(0.0f, 0.0f, -20.0f);

            if (enableRotate)
            {
                GL.glRotatef(viewAngle, 0.0f, 1.0f, 0.0f);
                viewAngle -= 2f;            
            }

            DrawRoom();
            DrawOldAxes();
            DrawLightSource();

            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, ModelViewMatrixBeforeTransforms);
            GL.glLoadIdentity();
          
            if (coordSysMoveDirection != 0)
            {
                delta = 5.0f * Math.Abs(coordSysMoveDirection) / coordSysMoveDirection;

                switch (Math.Abs(coordSysMoveDirection))
                {
                    case 1:
                        GL.glTranslatef(delta / 20, 0, 0);
                        break;
                    case 2:
                        GL.glTranslatef(0, delta / 20, 0);
                        break;
                    case 3:
                        GL.glTranslatef(0, 0, delta / 20);
                        break;
                    case 4:
                        GL.glRotatef(delta, 1, 0, 0);
                        break;
                    case 5:
                        GL.glRotatef(delta, 0, 1, 0);
                        break;
                    case 6:
                        GL.glRotatef(delta, 0, 0, 1);
                        break;
                }
            }

            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, CurrentTraslation);
            GL.glLoadMatrixd(AccumulatedTraslations);
            GL.glMultMatrixd(CurrentTraslation);
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedTraslations);
            GL.glLoadMatrixd(ModelViewMatrixBeforeTransforms);
            GL.glMultMatrixd(AccumulatedTraslations);

            DrawAxes();
            GL.glCallList(handList);

            if (enableReflection)
            {
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
                GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF);
                GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
                GL.glDisable(GL.GL_DEPTH_TEST);
              
                //DrawFloor();
                GL.glCallList(floorMainList);

                GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
                GL.glEnable(GL.GL_DEPTH_TEST);
                GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glPushMatrix();
                GL.glScalef(1, -1, 1);
                GL.glEnable(GL.GL_CULL_FACE);
                GL.glCullFace(GL.GL_BACK);

                GL.glTranslated(0, 3, 0);
                DrawFigures();

                GL.glCullFace(GL.GL_FRONT);

                DrawFigures();

                GL.glDisable(GL.GL_CULL_FACE);
                GL.glPopMatrix();
                GL.glDepthMask((byte)GL.GL_FALSE);

                //DrawFloor();
                GL.glCallList(floorMainList);

                GL.glDepthMask((byte)GL.GL_TRUE);
                GL.glDisable(GL.GL_STENCIL_TEST);
            }       

            GL.glFlush();
            WGL.wglSwapBuffers(m_uint_DC);
        }

        protected void DrawRoom()
        {
            GL.glColor3f(1.0f, 1.0f, 1.0f);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, texture[0]);
            GL.glDisable(GL.GL_LIGHTING);

            GL.glBegin(GL.GL_QUADS);

            GL.glTexCoord2d(1, 1);
            GL.glVertex3d(-100, 100, 100);
            GL.glTexCoord2d(0.75, 1);
            GL.glVertex3d(100, 100, 100);
            GL.glTexCoord2d(0.75, 0);
            GL.glVertex3d(100, -100, 100);
            GL.glTexCoord2d(1, 0);
            GL.glVertex3d(-100, -100, 100);

            GL.glTexCoord2d(0.25, 1);
            GL.glVertex3d(-100, 100, -100);
            GL.glTexCoord2d(0.5, 1);
            GL.glVertex3d(100, 100, -100);
            GL.glTexCoord2d(0.5, 0);
            GL.glVertex3d(100, -100, -100);
            GL.glTexCoord2d(0.25, 0);
            GL.glVertex3d(-100, -100, -100);

            GL.glTexCoord2d(0.25, 1);
            GL.glVertex3d(-100, 100, -100);
            GL.glTexCoord2d(0, 1);
            GL.glVertex3d(-100, 100, 100);
            GL.glTexCoord2d(0, 0);
            GL.glVertex3d(-100, -100, 100);
            GL.glTexCoord2d(0.25, 0);
            GL.glVertex3d(-100, -100, -100);

            GL.glTexCoord2d(0.75, 1);
            GL.glVertex3d(100, 100, 100);
            GL.glTexCoord2d(0.5, 1);
            GL.glVertex3d(100, 100, -100);
            GL.glTexCoord2d(0.5, 0);
            GL.glVertex3d(100, -100, -100);
            GL.glTexCoord2d(0.75, 0);
            GL.glVertex3d(100, -100, 100);

            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
        }

        protected void DrawFloor()
        {
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColor4d(0, 0, 1, 0.3);                      

            GL.glBegin(GL.GL_QUADS);

            GL.glVertex3d(-7, -4, 7);
            GL.glVertex3d(7, -4, 7);
            GL.glVertex3d(7, -4, -7);
            GL.glVertex3d(-7, -4, -7);

            GL.glEnd();
            GL.glDisable(GL.GL_LIGHTING);
        }

        protected void DrawLightSource()
        {
            if (enableLightSource)
            {               
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, lightPosition);              
                GL.glEnable(GL.GL_LIGHTING);
                GL.glEnable(GL.GL_COLOR_MATERIAL);

                GL.glTranslated(lightPosition[0], lightPosition[1], lightPosition[2]);
                GL.glColor3d(1, 1, 0);
                GLUT.glutSolidSphere(0.5, 50, 50);
                GL.glTranslated(-lightPosition[0], -lightPosition[1], -lightPosition[2]);
            }
            else
            {
                GL.glDisable(GL.GL_LIGHTING);
            }
        }

        protected void DrawAxes()
        {
            GL.glBegin(GL.GL_LINES);

            //X - Red
            GL.glColor3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(10.0f, 0.0f, 0.0f);

            //Y - Greed 
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 10.0f, 0.0f);

            //Z - Blue
            GL.glColor3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 10.0f);

            GL.glEnd();
        }

        protected void DrawOldAxes()
        {
            if (enableDefaultCoordSystem)
            {
                GL.glEnable(GL.GL_LINE_STIPPLE);
                GL.glLineStipple(1, 0xFF00);
                GL.glBegin(GL.GL_LINES);

                //X - Red
                GL.glColor3f(1.0f, 0.0f, 0.0f);
                GL.glVertex3f(0.0f, 0.0f, 0.0f);
                GL.glVertex3f(10.0f, 0.0f, 0.0f);

                //Y - Greed 
                GL.glColor3f(0.0f, 1.0f, 0.0f);
                GL.glVertex3f(0.0f, 0.0f, 0.0f);
                GL.glVertex3f(0.0f, 10.0f, 0.0f);

                //Z - Blue
                GL.glColor3f(0.0f, 0.0f, 1.0f);
                GL.glVertex3f(0.0f, 0.0f, 0.0f);
                GL.glVertex3f(0.0f, 0.0f, 10.0f);

                GL.glEnd();
                GL.glDisable(GL.GL_LINE_STIPPLE);
            }
        }

        protected void DrawHand()
        {
            GL.glColor3d(1, 0.8, 0.6);

            GL.glBegin(GL.GL_QUADS);

            //Near
            GL.glVertex3d(-3.0, 2.0, 0.5);
            GL.glVertex3d(3.0, 2.0, 0.5);
            GL.glVertex3d(3.0, -2.0, 0.5);
            GL.glVertex3d(-3.0, -2.0, 0.5);

            //Far
            GL.glVertex3d(-3.0, 2.0, -0.5);
            GL.glVertex3d(3.0, 2.0, -0.5);
            GL.glVertex3d(3.0, -2.0, -0.5);
            GL.glVertex3d(-3.0, -2.0, -0.5);

            //Left
            GL.glVertex3d(-3.0, 2.0, -0.5);
            GL.glVertex3d(-3.0, 2.0, 0.5);
            GL.glVertex3d(-3.0, -2.0, 0.5);
            GL.glVertex3d(-3.0, -2.0, -0.5);

            //Right
            GL.glVertex3d(3.0, 2.0, 0.5);
            GL.glVertex3d(3.0, 2.0, -0.5);
            GL.glVertex3d(3.0, -2.0, -0.5);
            GL.glVertex3d(3.0, -2.0, 0.5);

            GL.glEnd();
        }

        protected void DrawSingleFinger(int fingerPart, double returnToZero)
        {
            switch (fingerPart)
            {
                case 1:
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.6, 50, 50);
                    GL.glTranslated(0, 0.2, 0);
                    GL.glRotated(-90, 1, 0, 0);
                    GL.glColor3d(1, 0.8, 0.6);
                    GLU.gluCylinder(objFinger, 0.5, 0.5, 1, 50, 50);
                    GL.glRotated(90, 1, 0, 0);
                    GL.glTranslated(0, 0.8, 0);
                    break;

                case 2:
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.6, 50, 50);
                    GL.glTranslated(0, 0.2, 0);
                    GL.glRotated(-90, 1, 0, 0);
                    GL.glColor3d(1, 0.8, 0.6);
                    GLU.gluCylinder(objFinger, 0.5, 0.5, 0.7, 50, 50);
                    GL.glRotated(90, 1, 0, 0);
                    GL.glTranslated(0, 0.8, 0);
                    break;

                case 3:
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.6, 50, 50);
                    GL.glTranslated(0, 0.2, 0);
                    GL.glRotated(-90, 1, 0, 0);
                    GL.glColor3d(1, 0.8, 0.6);
                    GLU.gluCylinder(objFinger, 0.5, 0.5, 0.7, 50, 50);
                    GL.glRotated(90, 1, 0, 0);
                    GL.glTranslated(0, 0.6, 0);
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.5, 50, 50);
                    GL.glTranslated(-returnToZero, -5.1, 0);
                    break;
            }
        }
       
        protected void DrawThumbFinger(int fingerPart)
        {
            switch (fingerPart)
            {
                case 1:
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.6, 50, 50);
                    GL.glTranslated(0, 0.2, 0);
                    GL.glRotated(-90, 1, 0, 0);
                    GL.glColor3d(1, 0.8, 0.6);
                    GLU.gluCylinder(objFinger, 0.5, 0.5, 1, 50, 50);
                    GL.glRotated(90, 1, 0, 0);
                    GL.glTranslated(0, 0.8, 0);
                    GL.glRotated(45, 0, 0, 1);
                    break;

                case 2:
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.6, 50, 50);
                    GL.glTranslated(0, 0.2, 0);
                    GL.glRotated(-90, 1, 0, 0);
                    GL.glColor3d(1, 0.8, 0.6);
                    GLU.gluCylinder(objFinger, 0.5, 0.5, 0.7, 50, 50);
                    GL.glRotated(90, 1, 0, 0);
                    GL.glTranslated(0, 0.6, 0);
                    GL.glColor3d(1, 1, 1);
                    GLU.gluSphere(objShpere, 0.5, 50, 50);
                    GL.glRotated(45, 0, 0, 1);
                    GL.glTranslated(-4.6, -1.8, 0);
                    break;
            }
        }

        protected void CallFingerList(int fingerIndex)
        {
            switch (fingerIndex)
            {
                case 1:
                    GL.glPushMatrix();
                    GL.glCallList(indexFingerStart);
                    GL.glRotatef(indexFingerAngle[0], 1, 0, 0);
                    GL.glCallList(indexPhalanxList1);
                    GL.glRotatef(indexFingerAngle[1], 1, 0, 0);
                    GL.glCallList(indexPhalanxList2);
                    GL.glRotatef(indexFingerAngle[2], 1, 0, 0);
                    GL.glCallList(indexPhalanxList3);
                    GL.glPopMatrix();
                    break;

                case 2:
                    GL.glPushMatrix();
                    GL.glCallList(middleFingerStart);
                    GL.glRotatef(middleFingerAngle[0], 1, 0, 0);
                    GL.glCallList(middlePhalanxList1);
                    GL.glRotatef(middleFingerAngle[1], 1, 0, 0);
                    GL.glCallList(middlePhalanxList2);
                    GL.glRotatef(middleFingerAngle[2], 1, 0, 0);
                    GL.glCallList(middlePhalanxList3);
                    GL.glPopMatrix();
                    break;

                case 3:
                    GL.glPushMatrix();
                    GL.glCallList(ringFingerStart);
                    GL.glRotatef(ringFingerAngle[0], 1, 0, 0);
                    GL.glCallList(ringPhalanxList1);
                    GL.glRotatef(ringFingerAngle[1], 1, 0, 0);
                    GL.glCallList(ringPhalanxList2);
                    GL.glRotatef(ringFingerAngle[2], 1, 0, 0);
                    GL.glCallList(ringPhalanxList3);
                    GL.glPopMatrix();
                    break;

                case 4:
                    GL.glPushMatrix();
                    GL.glCallList(pinkyFingerStart);
                    GL.glRotatef(pinkyFingerAngle[0], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList1);
                    GL.glRotatef(pinkyFingerAngle[1], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList2);
                    GL.glRotatef(pinkyFingerAngle[2], 1, 0, 0);
                    GL.glCallList(pinkyPhalanxList3);
                    GL.glPopMatrix();
                    break;

                case 5:
                    GL.glPushMatrix();
                    GL.glCallList(thumbFingerStart);
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glRotatef(thumbFingerAngle[0], 1, 0, 0);
                    GL.glRotated(45, 0, 0, 1);
                    GL.glCallList(thumbPhalanxList1);
                    GL.glRotated(-45, 0, 0, 1);
                    GL.glRotatef(thumbFingerAngle[1], 1, 0, 0);
                    GL.glRotated(45, 0, 0, 1);
                    GL.glCallList(thumbPhalanxList2);
                    GL.glPopMatrix();
                    break;
            }
        }

        protected virtual void InitializeGL()
		{
			m_uint_HWND = (uint)p.Handle.ToInt32();
			m_uint_DC = WGL.GetDC(m_uint_HWND);

			WGL.wglSwapBuffers(m_uint_DC);

			WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
			WGL.ZeroPixelDescriptor(ref pfd);
			pfd.nVersion = 1; 
			pfd.dwFlags = (WGL.PFD_DRAW_TO_WINDOW |  WGL.PFD_SUPPORT_OPENGL |  WGL.PFD_DOUBLEBUFFER); 
			pfd.iPixelType = (byte)(WGL.PFD_TYPE_RGBA);
			pfd.cColorBits = 32;
			pfd.cDepthBits = 32;
			pfd.iLayerType = (byte)(WGL.PFD_MAIN_PLANE);
            pfd.cStencilBits = 32;

            int pixelFormatIndex = 0;
			pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);

			if(pixelFormatIndex == 0)
			{
				MessageBox.Show("Unable to retrieve pixel format");
				return;
			}

			if(WGL.SetPixelFormat(m_uint_DC,pixelFormatIndex,ref pfd) == 0)
			{
				MessageBox.Show("Unable to set pixel format");
				return;
			}

			m_uint_RC = WGL.wglCreateContext(m_uint_DC);

			if(m_uint_RC == 0)
			{
				MessageBox.Show("Unable to get rendering context");
				return;
			}

			if(WGL.wglMakeCurrent(m_uint_DC,m_uint_RC) == 0)
			{
				MessageBox.Show("Unable to make rendering context current");
				return;
			}

            initRenderingGL();
        }
    
        protected virtual void initRenderingGL()
		{
			if(m_uint_DC == 0 || m_uint_RC == 0)
				return;

            GL.glShadeModel(GL.GL_SMOOTH);
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);
            GL.glClearDepth(1.0f);

            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColorMaterial(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE);

            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_Hint, GL.GL_NICEST);

            GL.glViewport(0, 0, Width, Height);
            GL.glClearColor(0, 0, 0, 0); 
			GL.glMatrixMode (GL.GL_PROJECTION);
			GL.glLoadIdentity();
            GLU.gluPerspective(45.0, 1.0, 1.0, 1000.0);
            GL.glMatrixMode (GL.GL_MODELVIEW);
			GL.glLoadIdentity();
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedTraslations);

            InitTexture("SurgeryRoom.bmp");
        }        
    }
}