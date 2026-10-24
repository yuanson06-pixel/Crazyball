using Crazyball.CourseKit.Coins;
using TMPro;
using UnityEngine;

namespace Crazyball.CourseKit.UI
{
    public sealed class RunCoinText : MonoBehaviour
    {
        [SerializeField] private RunCoinWallet wallet;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string prefix = "Coin  ";

        private void OnEnable()
        {
            if (wallet == null)
                return;

            wallet.BalanceChanged += Refresh;
            Refresh(wallet.Balance);
        }

        private void OnDisable()
        {
            if (wallet != null)
                wallet.BalanceChanged -= Refresh;
        }

        private void Refresh(int value)
        {
            if (label != null)
                label.text = $"{prefix}{value}";
        }
    }

}
