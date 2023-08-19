namespace IGService3
{

    //-----------------------------------------------------------------------------------
    // MC: classe timer software
    //-----------------------------------------------------------------------------------
    public class CTimerSW
    {
        private int m_nTimeStop;
        private int m_nUltimoControllo;

        public void Start(int nMillisecondi)
        {
            m_nTimeStop = System.Environment.TickCount + nMillisecondi;
            m_nUltimoControllo = System.Environment.TickCount;
        }

        public bool TimerScaduto()
        {
            bool bReply = false;
            if (System.Environment.TickCount > m_nTimeStop) //è scaduto il timer
            {
                bReply = true;
            }
            else if (m_nUltimoControllo > System.Environment.TickCount)// ripartito tick sistema ...
                bReply = true;
            else
                m_nUltimoControllo = System.Environment.TickCount;
            return bReply;
        }
    }
}
