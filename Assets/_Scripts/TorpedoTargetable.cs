﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class TorpedoTargetable : MonoBehaviour
    {
        public event Action<Torpedo> OnHitByTorpedo;
        public event Action<Mine> OnHitByMine;

        public void Start()
        {
            SubManager.Instance.Targetable.Add(this);
        }

        public void OnDestroy()
        {
            SubManager.Instance.Targetable.Remove(this);
        }

        public void HitByTorpedo(Torpedo torpedo)
        {
            if (OnHitByTorpedo != null)
                OnHitByTorpedo(torpedo);
        }

        public void HitByMine(Mine mine)
        {
            if (OnHitByMine != null)
                OnHitByMine(mine);
        }
    }
}