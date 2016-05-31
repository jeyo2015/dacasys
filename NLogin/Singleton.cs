namespace NLogin
{
    using System;

    public class Singleton
    {
        static private Singleton singleton = null;
        String gDatos = "";
        String gResponse = "";
        bool gSW = false;
        private Singleton() { 
        
        }

        static public Singleton getSingleton()
        {

            if (singleton == null)
            {
                singleton = new Singleton();
            }
            return singleton;
        }

        public bool getgSW() {
            return gSW;
        }

        public void setgSW(bool value) {
            gSW = value;
        }

        public String getDatos()
        {
            return gDatos;
        }

        public void setDatos(string pDatos) {
            gDatos = pDatos;
        }

        public String getResponse()
        {
            return gResponse;
        }

        public void setResponse(string pResponse)
        {
            gResponse = pResponse;
        }
    }
}