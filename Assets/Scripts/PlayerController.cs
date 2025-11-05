using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float minLookAngle;
    [SerializeField] private float maxLookAngle;

    [SerializeField] private LayerMask whatIsStock;
    [SerializeField] private float interactionRange;

    [SerializeField] private Transform holdPoint;

    [SerializeField] private float throwForce;

    [SerializeField] private LayerMask whatIsShelf;

    [SerializeField] private LayerMask whatIsStockBox;
    [SerializeField] private Transform boxHoldPoint;

    [SerializeField] private float waitToPlaceStock;
    [SerializeField] private LayerMask whatIsTrash;
    [SerializeField] private LayerMask whatIsFurniture;
    [SerializeField] private Transform furniturePoint;
    private float placeStockCounter;
    private StockBoxController heldBox;
    private FurnitureController heldFurniture;
    

    private StockObject heldPickup;

    private Camera cam;
    private CharacterController charCon;
    private float ySpeed;
    private float horRot;
    private float vertRot;


    private void Awake()
    {
        charCon = GetComponent<CharacterController>();
        cam = Camera.main;
    }
    
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (UIController.instance.updatePricePanel != null)
        {
            if (UIController.instance.updatePricePanel.activeSelf)
            {
                return;
            }
        }
        if(UIController.instance.buyMenuScreen != null)
        {
            if(UIController.instance.buyMenuScreen.activeSelf)
            {
                return;
            }
        }
        CharLook();
        CharMove();
        CheckForPickup();
    }

    private void CharLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        horRot += lookInput.x * Time.deltaTime * lookSpeed;

        transform.rotation = Quaternion.Euler(0f, horRot, 0f);

        vertRot -= lookInput.y * Time.deltaTime * lookSpeed;
        vertRot = Mathf.Clamp(vertRot, minLookAngle, maxLookAngle);

        cam.transform.localRotation = Quaternion.Euler(vertRot, 0f, 0f);
    }

    private void CharMove()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        //Vector3 moveAmount = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 vertMove = transform.forward * moveInput.y;
        Vector3 horMove = transform.right * moveInput.x;

        Vector3 moveAmount = horMove + vertMove;
        moveAmount = moveAmount.normalized;

        moveAmount += moveAmount * moveSpeed;

        if (charCon.isGrounded)
        {
            ySpeed = 0f;
            if (jumpAction.action.WasPressedThisFrame())
            {
                ySpeed = jumpForce;
            }
        }

        ySpeed = ySpeed + (Physics.gravity.y * Time.deltaTime);



        moveAmount.y = ySpeed;

        charCon.Move(moveAmount * Time.deltaTime);
    }

    private void CheckForPickup()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if(heldPickup == null && heldBox == null && heldFurniture == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    if(hit.collider.GetComponent<StockObject>() != null)
                    {
                        heldPickup = hit.collider.GetComponent<StockObject>();
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.Pickup();
                    }

                    return;
                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    if(hit.collider.GetComponent<StockBoxController>())
                    {
                        heldBox = hit.collider.GetComponent<StockBoxController>();
                        heldBox.transform.SetParent(boxHoldPoint);
                        heldBox.Pickup();


                        if (!heldBox.openBox.activeSelf)
                        {
                            heldBox.OpenClose();
                        }
                    }

                    return;

                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    if(hit.collider.GetComponent<ShelfSpaceController>() != null)
                    {
                        heldPickup = hit.collider.GetComponent<ShelfSpaceController>().GetStock();

                        if (heldPickup != null)
                        {
                            heldPickup.transform.SetParent(holdPoint);
                            heldPickup.Pickup();
                        }
                    }
                    
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    if(hit.collider.GetComponent<StockBoxController>() != null)
                        hit.collider.GetComponent<StockBoxController>().OpenClose();
                }
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    if (hit.collider.GetComponent<ShelfSpaceController>() != null)
                        hit.collider.GetComponent<ShelfSpaceController>().StartPriceUpdate();
                }
            }
            
            if(Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsFurniture))
                {
                    heldFurniture = hit.transform.GetComponent<FurnitureController>();

                    heldFurniture.transform.SetParent(furniturePoint);
                    heldFurniture.transform.localPosition = Vector3.zero;
                    heldFurniture.transform.localRotation = Quaternion.identity;

                    heldFurniture.MakePlaceable();

                }
            }
        } else
        {
            if(heldPickup != null)
            {
                if(Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                    {
                        if(hit.transform.GetComponent<ShelfSpaceController>() != null)
                        {
                            hit.transform.GetComponent<ShelfSpaceController>().PlaceStock(heldPickup);
                            if(heldPickup.GetIsPlaced())
                            {
                                heldPickup = null;
                            }
                        }
                        
                    }
                }

                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    heldPickup.Release();
                    heldPickup.GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

                    heldPickup.transform.SetParent(null);
                    heldPickup = null;
                }
            }
            if (heldBox != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (heldBox.GetStockInBoxCount() > 0)
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                        {
                            heldBox.PlaceStockOnShelf(hit.collider.GetComponent<ShelfSpaceController>());

                            placeStockCounter = waitToPlaceStock;
                        }
                    }
                    else
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsTrash))
                        {
                            Destroy(heldBox.gameObject);
                            heldBox = null;
                        }
                    }


                }
                if (Mouse.current.leftButton.isPressed)
                {
                    placeStockCounter -= Time.deltaTime;
                    if (placeStockCounter <= 0)
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                        {
                            heldBox.PlaceStockOnShelf(hit.collider.GetComponent<ShelfSpaceController>());

                            placeStockCounter = waitToPlaceStock;
                        }
                    }
                }
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    if (heldBox.openBox.activeSelf)
                    {
                        heldBox.OpenClose();
                    }

                    heldBox.Release();
                    heldBox.GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

                    heldBox.transform.SetParent(null);
                    heldBox = null;
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    heldBox.OpenClose();
                }
            }
            
            if(heldFurniture != null)
            {
                heldFurniture.transform.position = new Vector3(furniturePoint.position.x, 0f, furniturePoint.position.z);
                heldFurniture.transform.LookAt(new Vector3(transform.position.x, 0f, transform.position.z));

                if(Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame)
                {
                    heldFurniture.transform.SetParent(null);

                    heldFurniture.PlaceFurniture();
                    
                    heldFurniture = null;
                }
            }
        }

        
    }
}
