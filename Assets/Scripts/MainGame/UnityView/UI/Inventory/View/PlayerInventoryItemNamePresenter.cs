using Core.Const;
using MainGame.UnityView.UI.Inventory.Control;
using MainGame.UnityView.UI.Inventory.Element;
using TMPro;
using UnityEngine;
using VContainer;

namespace MainGame.UnityView.UI.Inventory.View
{
    public class PlayerInventoryItemNamePresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private GameObject itemNameTextGameObject;
        [SerializeField] private PlayerInventorySlots playerInventorySlots;
        private ItemImages _itemImages;
        
        private PlayerInventoryViewModel _playerInventoryViewModel;
        private PlayerInventoryViewModelController _playerInventoryViewModelController;
        

        [Inject]
        public void Construct(PlayerInventoryViewModel playerInventoryViewModel,PlayerInventoryViewModelController playerInventoryViewModelController,ItemImages itemImages)
        {
            _playerInventoryViewModel = playerInventoryViewModel;
            _playerInventoryViewModelController = playerInventoryViewModelController;
            _itemImages = itemImages;
            playerInventorySlots.OnCursorEnter += OnCursorEnter;
            playerInventorySlots.OnCursorExit += _ => itemNameTextGameObject.SetActive(false);
        }

        private void OnCursorEnter(int slot)
        {
            if (_playerInventoryViewModelController.IsGrabbed)return;
            
            var item = _playerInventoryViewModel[slot];
            
            if (item.Count == ItemConst.EmptyItemId)
            {
                itemNameText.text = "";
                itemNameTextGameObject.SetActive(false);
                return;
            }
            itemNameText.text = _itemImages.GetItemView(item.Id).itemName;
            itemNameTextGameObject.SetActive(true);
        }
    }
    
}