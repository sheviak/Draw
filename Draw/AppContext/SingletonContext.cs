namespace Draw.AppContext
{
    public class SingletonContext
    {
        public static ApplicationContext Context = null;
        private static readonly object Instancelock = new object();
        private SingletonContext()
        {
            Context = new ApplicationContext();
        }

        private static SingletonContext instance = null;
        public static SingletonContext GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (Instancelock)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonContext();
                        }
                    }
                }
                return instance;
            }
        }
    }
}