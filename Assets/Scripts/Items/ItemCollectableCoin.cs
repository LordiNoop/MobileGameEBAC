using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectableCoin : ItemCollectableBase
{
    public Collider coll;
    public bool collect = false;
    public float lerp = 5f;
    public float minDistance = 1f;

    private void Start()
    {
        CoinsAnimatorManager.Instance.RegisterCoin(this);
    }

    private void Update()
    {
        if (collect)
        {
            transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position, lerp * Time.deltaTime);

            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < minDistance)
            {
                //HideItems();
                Destroy(gameObject);
            }
        }
    }

    protected override void Collect()
    {
        OnCollect();
    }

    protected override void OnCollect()
    {
        base.OnCollect();
        //coll.enabled = false;
        collect = true;
        PlayerController.Instance.Bounce();
    }
}
