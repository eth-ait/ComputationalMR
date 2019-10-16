using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class FadeDecorator : MonoBehaviour
    {
        private bool _started;
        private ElementModel _element;

        void Update()
        {
            if (!_started)
            {
                _started = true;
                _element = GetComponentInParent<ElementModel>();

                if (_element != null)
                    PersistState();
            }

            if (_element == null)
                return;

            CheckStateChanged();
        }

        private void PersistState()
        {
            
        }

        private void CheckStateChanged()
        {

        }
    }
}