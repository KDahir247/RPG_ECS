using System;
using UniRx;
using Unity.Entities;

namespace WorldOfECS.Event
{
    //Not Done
    public class AttributeCommand<T> where T : struct,IComponentData, IEquatable<T>
    {
        private readonly Entity _entity;
        private EntityManager _dstManager;
        
        private readonly ReactiveProperty<T> _reactiveAttribute = new ReactiveProperty<T>(default);

        private ReactiveCommand _reactiveAttributeCommand;
        
        public AttributeCommand(Entity entity, EntityManager manager)
        {
            _entity = entity;
            _dstManager = manager;
        }
        
        
        public ReactiveCommand Thin(Func<T, bool> gauntlet)
        {
            if (!_dstManager.GetComponentData<T>(_entity).Equals(_reactiveAttribute.Value))
            {
                _reactiveAttribute.Value = _dstManager.GetComponentData<T>(_entity);
            }
            
            _reactiveAttributeCommand = _reactiveAttribute.Select(gauntlet).ToReactiveCommand();

            return _reactiveAttributeCommand;
        }

        /// <summary>
        /// Called when the att
        /// </summary>
        /// <returns></returns>
        public IObservable<T> ObserverChange()
        {
            return _reactiveAttribute.ObserveEveryValueChanged(x => x.Value);
        }
        

    }
}
