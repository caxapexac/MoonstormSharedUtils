﻿using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Moonstorm
{
    /// <summary>
    /// A <see cref="MaterialCopier"/> is a ScriptableObject that allows the usage of vanilla materials in your project.
    /// </summary>
    [Obsolete("Set your material's shader to MSU's AddressableMaterialShader.")]
    public class MaterialCopier : ScriptableObject
    {
        /// <summary>
        /// A Representation of a in-project material, and the address that the in-project material will use ingame.
        /// </summary>
        [Serializable]
        public class MaterialPair
        {
            [Tooltip("The address of the material, the properties and shader of this material will be used on the material below.")]
            public string materialAddress;
            [Tooltip("The material in your project, the properties and shader of this material will be copied from the material address")]
            public Material material;
        }
        private static readonly List<MaterialCopier> instances = new List<MaterialCopier>();
        private static readonly List<Material> copiedMaterials = new List<Material>();

        [SystemInitializer]
        private static void Initialize()
        {
            MSULog.Info($"Material Copier Initialized");
            foreach (MaterialCopier copier in instances)
            {
#if DEBUG
                MSULog.Debug($"Copying materials from {copier}");
#endif
                copier.CopyMaterials();
            }
        }

        [Tooltip("The material pairs for this material copier")]
        public List<MaterialPair> materialPairs = new List<MaterialPair>();

        private void Awake()
        {
            instances.AddIfNotInCollection(this);
        }

        private void OnDestroy()
        {
            instances.RemoveIfInCollection(this);
        }

#if DEBUG
        [ContextMenu("Upgrade to AddressableMaterialShader")]
        private void Upgrade()
        {
            foreach (MaterialPair pair in materialPairs)
            {
                UpgradeSingle(pair.material, pair.materialAddress);
            }
            Debug.Log("Upgrade finished, remember to call \"FinalizeMaterialsWithAddressableMaterialShader\" method in your AssetsLoader!");
        }
#endif

        private void UpgradeSingle(Material material, string address)
        {
#if DEBUG
            Debug.Log($"Upgraded {material} to use AddressableMaterialShader");
#endif
            var shader = Shader.Find("AddressableMaterialShader");
            material.shader = shader;
            material.shaderKeywords = new string[] { address };
        }

        private void CopyMaterials()
        {
            foreach (MaterialPair pair in materialPairs)
            {
                try
                {
                    CopyFromMaterialAddress(pair);
                }
                catch (Exception ex)
                {
                    MSULog.Error(ex);
                }
            }
        }

        private async void CopyFromMaterialAddress(MaterialPair materialPair)
        {
            var asyncOp = Addressables.LoadAssetAsync<Material>(materialPair.materialAddress);
            var task = asyncOp.Task;
            var originalMaterial = await task;
            materialPair.material.shader = originalMaterial.shader;
            materialPair.material.CopyPropertiesFromMaterial(originalMaterial);
            copiedMaterials.Add(materialPair.material);
#if DEBUG
            MSULog.Debug($"Properties from {originalMaterial} copied to {materialPair.material}");
#endif
        }
    }
}
