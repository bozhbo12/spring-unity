namespace UnityDevelopment.Calculator
{
    /// <summary>
    /// volumn control, the volumn will never exceeded max value
    /// </summary>
    public class Volumn
    {
        private float currVol;
        private float maxVol;

        /// <summary>
        /// current volume value
        /// </summary>
        public float currentVolumn { 
            get 
            { 
                return currVol; 
            }
            set
            {
                currVol = LinearData.nonrecurrentValidate(value, maxVol, 0);
            }
        }
        /// <summary>
        /// max volumn value
        /// </summary>
        public float maxVolumn { 
            get { 
                return maxVol; 
            }
            set
            {
                maxVol = value;
                currVol = LinearData.nonrecurrentValidate(currVol, maxVol, 0);
            }
        }
        /// <summary>
        /// the volumn is minimized
        /// </summary>
        public bool isEmpty { get { return currVol == 0; } }
        /// <summary>
        /// the volumn is maximized
        /// </summary>
        public bool isFull { get { return currVol == maxVol; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="maxVolumn">max volumn</param>
        /// <param name="isEmpty">if true, volumn will initialy empty, otherwise opposite</param>
        public Volumn(float maxVolumn, bool isEmpty)
        {
            this.maxVol = maxVolumn;
            if(isEmpty)
                currVol = 0;
            else
                currVol = this.maxVol;
        }
    }
}