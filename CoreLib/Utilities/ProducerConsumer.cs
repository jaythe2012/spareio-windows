using System;
using System.Collections.Generic;
using System.Threading;

namespace CoreLib
{
    public class ProducerConsumerQueue<T> : IDisposable
    {
        private EventWaitHandle _wh = new AutoResetEvent(false);
        private EventWaitHandle _consumerReady = new AutoResetEvent(false);
        private readonly object _locker = new object();
        private Queue<T> _tasks = new Queue<T>();
        private readonly Action<T> _consumerHandler;
        private readonly Thread _worker;
        private long _isConsumerReady;

        public ProducerConsumerQueue(Action<T> consumerHandler, bool isConsumerReady = true, string threadName = null, bool isBackground = true)
        {
            if (consumerHandler == null)
            {
                throw new ArgumentNullException("parameter handler cannot be null");
            }
            _consumerHandler = consumerHandler;
            _isConsumerReady = isConsumerReady ? 1 : 0;
            _worker = new Thread(Work);
            _worker.Name = threadName;
            _worker.IsBackground = isBackground;
            _worker.Start();
        }

        public bool IsConsumerReady
        {
            set
            {
                long initialValue = _isConsumerReady;
                long newValue = value ? 1 : 0;
                if (Interlocked.CompareExchange(ref _isConsumerReady, newValue, initialValue) == initialValue
                    && newValue == 1)
                {
                    _consumerReady.Set();
                }

            }
            get
            {
                return _isConsumerReady == 1;
            }
        }
        public void EnqueueTask(T task)
        {
            lock (_locker)
            {
                _tasks.Enqueue(task);
            }
            _wh.Set();
        }

        public void Dispose()
        {
            EnqueueTask(default(T));// Signal the consumer to exit.
            _worker.Join();// Wait for the consumer's thread to finish.
            _wh.Close();// Release any OS resources.
            _consumerReady.Close();
        }

        public void ClearQueue()
        {
            lock (_locker)
            {
                _tasks.Clear();
            }
        }

        private void Work(object state)
        {
            while (true)
            {
                if (Interlocked.Read(ref _isConsumerReady) == 0)
                {
                    _consumerReady.WaitOne();
                }
                T task = default(T);
                bool hasTask = false;
                lock (_locker)
                {
                    if (_tasks.Count > 0)
                    {
                        hasTask = true;
                        task = _tasks.Dequeue();
                        if (EqualityComparer<T>.Default.Equals(task, default(T)))
                        {
                            return;
                        }
                    }
                }
                if (hasTask)
                {
                    _consumerHandler(task);
                }
                else
                {
                    _wh.WaitOne();// No more tasks - wait for a signal
                }
            }
        }
    }
}
