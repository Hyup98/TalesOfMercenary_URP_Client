using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [Tooltip("������ ���� + ������ ��ġ�� ������ ���̾�")]
    [SerializeField] private LayerMask player1Layer;
    [SerializeField] private LayerMask player2Layer;
    
    private Unit clickedUnit;
    private LayerMask controlLayer;

    public void SetPlayerMouseLayer(bool isReversed)
    {
        controlLayer = !isReversed ? player1Layer : player2Layer;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, controlLayer))
            {
                if (hit.collider.TryGetComponent(out Unit currentUnit))
                {
                    clickedUnit = currentUnit;
                    currentUnit.IsClicked = true;
                }
                else if (clickedUnit != null)
                {
                    clickedUnit.PointMove(hit.point);
                    clickedUnit.IsClicked = false;
                    clickedUnit = null;
                }
            }
            //else clickedUnit.IsClicked = false;
        }
    }
}
