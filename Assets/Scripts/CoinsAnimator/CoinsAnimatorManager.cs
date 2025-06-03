using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ebac.Core.Singleton;
using DG.Tweening;
using System.Linq;

public class CoinsAnimatorManager : Singleton<CoinsAnimatorManager>
{
    public List<ItemCollectableCoin> items;

    [Header("Animation")]
    public float scaleDuration = .2f;
    public float scaletimeBetweenPieces = .1f;
    public Ease ease = Ease.OutBack;

    private void Start()
    {
        items = new List<ItemCollectableCoin>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartAnimations();
        }
    }

    public void RegisterCoin(ItemCollectableCoin i)
    {
        if (!items.Contains(i))
        {
            items.Add(i);
            i.transform.localScale = new Vector3(.1f, .1f, .1f);
        }
    }

    public void StartAnimations()
    {
        StartCoroutine(ScalePiecesByTime());
    }

    IEnumerator ScalePiecesByTime()
    {
        foreach (var p in items)
        {
            p.transform.localScale = new Vector3(.1f, .1f, .1f);
        }

        Sort();

        yield return null;

        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.DOScale(1, scaleDuration).SetEase(ease);
            yield return new WaitForSeconds(scaletimeBetweenPieces);
        }
    }

    private void Sort()
    {
        items = items.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
    }
}
