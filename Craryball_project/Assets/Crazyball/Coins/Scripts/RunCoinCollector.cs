using UnityEngine;

namespace Crazyball.CourseKit.Coins
{
    public sealed class RunCoinCollector : MonoBehaviour
    {
        [SerializeField] private RunCoinWallet wallet;

        public void Collect(int amount)
        {
            if (wallet == null)
            {
                Debug.LogError("RunCoinCollector requires a RunCoinWallet reference.", this);
                return;
            }

            wallet.Add(amount);
        }
    }
}
