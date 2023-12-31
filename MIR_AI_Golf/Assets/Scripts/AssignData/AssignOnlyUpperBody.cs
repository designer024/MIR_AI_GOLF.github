using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EthanLin.Config;
using EthanLin.Variation;

namespace EthanLin.AssignDataHelper
{
    public class AssignOnlyUpperBody : MonoBehaviour
    {
        [SerializeField] private VariationConfig _variationConfig;
        
        /// <summary>
        /// 0: Miku, 
        /// 1: 死侍, 
        /// 2: 失敗的人, 
        /// 3: Leonard
        /// </summary>
        [Tooltip("0: Miku,\n1: 死侍,\n2: 失敗的人\n3: Leonard")]
        [SerializeField] private int _roleId;
        
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [SerializeField] private Material[] _fullBodyMaterials;
        [SerializeField] private Material[] _upperbodyMaterials;

        #region For Leonard

        [Header("Leonard Body")] [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererBody;
        [Header("Leonard Collar")] [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererCollar;
        [Header("Leonard Sweater")] [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererSweater;
        
        [Header("Leonard Pants")] [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererPants;
        [Header("Leonard Shoes")] [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererShoes;

        // [SerializeField] private GameObject LeonardShoes, LeonardPants;
        
        #endregion

        private void Start()
        {
            if (_variationConfig == null)
            {
                _variationConfig = GameObject.FindWithTag("VectorVariationManager").GetComponent<VariationConfig>();
            }
            
            SetBodyIsFullOrOnlyUpper(_variationConfig.GetOnlyUpperBody);
        }

        /// <summary>
        /// 設定全身 或 只有上半身
        /// </summary>
        public void SetBodyIsFullOrOnlyUpper(bool aIsOnlyUpper)
        {
            // Miku
            if (_roleId == 0)
            {
                var materials = _skinnedMeshRenderer.materials;
                materials[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                _skinnedMeshRenderer.materials = materials;
                // Debug.Log($"{AllConfigs.DEBUG_TAG}, Miku: {aIsOnlyUpper}");
            }
            // 死侍
            else if (_roleId == 1)
            {
                var materials = _skinnedMeshRenderer.materials;
                materials[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                materials[1] = aIsOnlyUpper ? _upperbodyMaterials[1] : _fullBodyMaterials[1];
                _skinnedMeshRenderer.materials = materials;
                // Debug.Log($"{AllConfigs.DEBUG_TAG}, 死侍: {aIsOnlyUpper}");
            }
            // 失敗的人
            else if (_roleId == 2)
            {
                var materials = _skinnedMeshRenderer.materials;
                materials[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                _skinnedMeshRenderer.materials = materials;
                // Debug.Log($"{AllConfigs.DEBUG_TAG}, 失敗的人: {aIsOnlyUpper}");
            }
            // Leonard
            else if (_roleId == 3)
            {
                var materialsBody = _skinnedMeshRendererBody.materials;
                var materialsCollar = _skinnedMeshRendererCollar.materials;
                var materialsSweater = _skinnedMeshRendererSweater.materials;
                var materialsPants = _skinnedMeshRendererPants.materials;
                var materialsShoes = _skinnedMeshRendererShoes.materials;
                
                materialsBody[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                materialsCollar[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                materialsSweater[0] = aIsOnlyUpper ? _upperbodyMaterials[0] : _fullBodyMaterials[0];
                materialsPants[0] = aIsOnlyUpper ? _upperbodyMaterials[1] : _fullBodyMaterials[0];
                materialsShoes[0] = aIsOnlyUpper ? _upperbodyMaterials[1] : _fullBodyMaterials[0];

                _skinnedMeshRendererBody.materials = materialsBody;
                _skinnedMeshRendererCollar.materials = materialsCollar;
                _skinnedMeshRendererSweater.materials = materialsSweater;
                _skinnedMeshRendererPants.materials = materialsPants;
                _skinnedMeshRendererShoes.materials = materialsShoes;

                // LeonardShoes.SetActive(!aIsOnlyUpper);
                // LeonardPants.SetActive(!aIsOnlyUpper);
                // Debug.Log($"{AllConfigs.DEBUG_TAG}, Leonard: {aIsOnlyUpper}");
            }
        }
    }
}