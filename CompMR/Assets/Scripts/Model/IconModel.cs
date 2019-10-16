using UnityEngine;

namespace Assets.Scripts.Model
{
    public class IconModel : MonoBehaviour
    {
        public int IconIndex;
        public int Row;
        public int Col;

        public GameObject IndicatorError;
        public GameObject IndicatorSuccess;

        public void DisableIndicators()
        {
            IndicatorSuccess.SetActive(false);
            IndicatorError.SetActive(false);
        }

        public void IndicateSuccess()
        {
            IndicatorSuccess.SetActive(true);
            IndicatorError.SetActive(false);
        }

        public void IndicateError()
        {
            IndicatorSuccess.SetActive(false);
            IndicatorError.SetActive(true);
        }
    }
}
