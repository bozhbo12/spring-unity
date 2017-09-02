
using SystemHelper;
namespace UnityDevelopment.Calculator
{
    public class TimedPulser
    {
        private NcTimerTool timer = new NcTimerTool();
        private float period;

        public TimedPulser(float period)
        {
            this.period = period;
            timer.Start();
        }

        public bool pulse()
        {
            bool flag = timer.GetTime() > period;
            if(flag)
                timer.Start();
            return flag;
        }
    }
}
