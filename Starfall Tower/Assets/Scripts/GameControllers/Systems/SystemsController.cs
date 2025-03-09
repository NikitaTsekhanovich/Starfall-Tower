using System;
using System.Collections.Generic;
using GameControllers.Systems.Properties;
using UnityEngine;

namespace GameControllers.Systems
{
    public class SystemsController : MonoBehaviour
    {
        private readonly List<IHaveUpdate> _updatesSystems = new ();
        private readonly List<IDisposable> _disposables = new ();

        public void RegistrationUpdateSystem(IHaveUpdate updateSystem) => _updatesSystems.Add(updateSystem);
        public void RegistrationDisposable(IDisposable disposable) => _disposables.Add(disposable);

        private void Update()
        {
            foreach (var updateSystem in _updatesSystems)
                updateSystem.Update();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            
            _updatesSystems.Clear();
            _disposables.Clear();
        }
    }
}
