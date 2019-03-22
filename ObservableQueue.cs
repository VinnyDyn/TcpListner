using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MyHttpListener
{
    public class ObservableQueue<T> : Queue, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly Queue<T> queue = new Queue<T>();

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add, item));
        }

        public T Dequeue()
        {
            var item = queue.Dequeue();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}