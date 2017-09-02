namespace UnityDevelopment.Calculator
{
    /// <summary>
    /// normal linear data helper class 
    /// </summary>
    public static class LinearData
    {
        /// <summary>
        /// data that will occur recurrent in certain range
        /// </summary>
        /// <param name="content">current value</param>
        /// <param name="MAX">max value</param>
        /// <param name="MIN">min value</param>
        /// <returns>validated value</returns>
        public static float recurrentValidate(float content, float MAX, float MIN)
        {
            while (content > MAX)
                content -= (MAX - MIN + 1);
            while (content < MIN)
            {
                content += (MAX - MIN + 1);
            }
            return content;
        }

        /// <summary>
        /// nature value in a certain range
        /// </summary>
        /// <param name="content">current value</param>
        /// <param name="MAX">max value</param>
        /// <param name="MIN">min value</param>
        /// <returns>validated value</returns>
        public static float nonrecurrentValidate(float content, float MAX, float MIN)
        {
            if (content > MAX)
                content = MAX;
            if (content < MIN)
            {
                content = MIN;
            }
            return content;
        }

        public static bool isInRange(float content, float MAX, float MIN)
        {
            return content < MAX && content > MIN;
        }

        public static bool isInRangeWithBoundary(float content, float MAX, float MIN)
        {
            return content <= MAX && content >= MIN;
        }

        public static bool IsNullOrEmpty(string s)
        {
            if (s == null)
                return true;
            foreach (char c in s)
            {
                if (c != ' ')
                    return false;
            }
            return true;
        }
    }
}
