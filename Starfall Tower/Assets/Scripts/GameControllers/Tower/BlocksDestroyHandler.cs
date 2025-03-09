using System.Collections;
using GameControllers.Entities.Blocks;
using UnityEngine;

namespace GameControllers.Tower
{
    public class BlocksDestroyHandler : MonoBehaviour
    {
        private bool _canDestroy;

        private void Start()
        {
            StartCoroutine(DelayBeforeDestroy());
        }

        private IEnumerator DelayBeforeDestroy()
        {
            yield return new WaitForSeconds(1f);
            _canDestroy = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Block block) && _canDestroy)
            {
                block.DoDestroy();
            }
        }
    }
}
