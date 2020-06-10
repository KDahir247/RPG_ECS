using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace WOrldOfECS.Event
{
    public class BehaviourEventBus : IObservable<UnityEngine.Behaviour>
    {
        private static readonly BehaviorSubject<UnityEngine.Behaviour> InternalEventCall 
            = new BehaviorSubject<Behaviour>(null);
        
        private static readonly CompositeDisposable _disposable = new CompositeDisposable();

        public static bool IsClear => _disposable.IsDisposed;

        /// <summary>
        /// Return IDisposable so you can dispose of the event when needed
        /// similar to -= delegate
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<UnityEngine.Behaviour> observer)
        { 
            return InternalEventCall.Subscribe(observer).AddTo(_disposable);
        }

        public void AddToBus(UnityEngine.Behaviour behaviour)
        {
            InternalEventCall.OnNext(behaviour);
        }


        /// <summary>
        /// Remove all events cached to the event bus.
        /// </summary>
        public void ClearEventBusCache()
        {
            if (!_disposable.IsDisposed)
                _disposable.Dispose();
        }

    }
}