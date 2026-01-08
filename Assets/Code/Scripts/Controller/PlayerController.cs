using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference escapeAction;

    [SerializeField] private Camera playerCam;

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
    [SerializeField] private LayerMask whatIsCheckout;

    [SerializeField] private LayerMask whatIsDoor;
    [SerializeField] private LayerMask whatIsSignOpen;
    [SerializeField] private LayerMask whatIsCheckoutStock;
    [SerializeField] private LayerMask whatIsShopName;
    [SerializeField] private LayerMask whatIsEnvironment;
    [SerializeField] private string whatIsMopActionTag;
    [SerializeField] private string whatIsBroomActionTag;

    public Transform mopHand;
    public Transform broomHand;
    public Transform brushHand;
    private float placeStockCounter;
    private StockBoxController heldBox;
    private FurnitureController heldFurniture;
    private Transform mopObj;
    private Transform broomObj;
    private Transform brushObj;
    private bool mopClean;
    private bool broomClean;
    private GameObject mopTrashElement;
    private GameObject broomTrashElement;


    private StockObject heldPickup;

    private Checkout checkOutElement;

    private Camera cam;
    private CharacterController charCon;
    private float ySpeed;
    private float horRot;
    private float vertRot;


    private void Awake()
    {
        instance = this;

        charCon = GetComponent<CharacterController>();
        cam = Camera.main;
    }
    
    private void Start() {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetMopObj(Transform mopObjAdd)
    {
        mopObj = mopObjAdd;
    }

    public void SetBroomObj(Transform broomObjAdd)
    {
        broomObj = broomObjAdd;
    }

    public void SetBrushObj(Transform brushObjAdd)
    {
        brushObj = brushObjAdd;
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
        if (UIController.instance.buyMenuScreen != null)
        {
            if (UIController.instance.buyMenuScreen.activeSelf)
            {
                return;
            }
        }
        if(UIController.instance.mapMenuScreen != null)
        {
            if (UIController.instance.mapMenuScreen.activeSelf)
            {
                return;
            }
        }

        if(UIController.instance.wheelTools != null)
        {
            if(UIController.instance.wheelTools.activeSelf)
            {
                return;
            }
        }

        if (escapeAction.action.WasPressedThisFrame() && !playerCam.gameObject.activeSelf)
        {
            CloseCheckout();
        }
        
        if (!playerCam.gameObject.activeSelf)
            return;
            
        CharLook();
        CharMove();
        CheckForPickup();
    }

    public void CloseCheckout()
    {
        playerCam.gameObject.SetActive(true);
        checkOutElement.DesactivateCam();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UIController.instance.TooglePlayerDot();
        checkOutElement = null;
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
        if (heldPickup == null && heldBox == null && heldFurniture == null && mopObj == null && broomObj == null && brushObj == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsCheckoutStock))
                {
                    StockObject obj = hit.collider.GetComponent<StockObject>();

                    if (obj != null)
                    {
                        if (Checkout.instance.customersInQueue.Count > 0)
                        {
                            obj.OutCheckout();
                            Checkout.instance.customersInQueue[0].GrabCheckout(obj);
                            Checkout.instance.UpdateScreen(obj);

                            Checkout.instance.RemoveObjectFromQueue(obj);

                            Checkout.instance.UpdateObjectsQueue();
                        }
                    }
                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    if (hit.collider.GetComponent<StockObject>() != null)
                    {
                        heldPickup = hit.collider.GetComponent<StockObject>();
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.Pickup();
                    }

                    return;
                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    if (hit.collider.GetComponent<StockBoxController>())
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
                    if (hit.collider.GetComponent<ShelfSpaceController>() != null)
                    {
                        heldPickup = hit.collider.GetComponent<ShelfSpaceController>().GetStock();

                        if (heldPickup != null)
                        {
                            heldPickup.transform.SetParent(holdPoint);
                            heldPickup.Pickup();
                        }
                    }

                    return;
                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsCheckout))
                {
                    //hit.collider.GetComponent<Checkout>().CheckoutCustomer();
                    playerCam.gameObject.SetActive(false);
                    checkOutElement = hit.collider.GetComponent<Checkout>();
                    checkOutElement.ActiveCam();
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    UIController.instance.TooglePlayerDot();
                }
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsSignOpen))
                {
                    //TODO action OpenStore
                    StoreController.instance.OpenStore();
                }
                
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    if (hit.collider.GetComponent<StockBoxController>() != null)
                        hit.collider.GetComponent<StockBoxController>().OpenClose();
                }
            }

            if (interactAction.action.WasPressedThisFrame())
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    if (hit.collider.GetComponent<ShelfSpaceController>() != null)
                        hit.collider.GetComponent<ShelfSpaceController>().StartPriceUpdate();
                }

                if (Physics.Raycast(ray, out hit, interactionRange, whatIsDoor))
                {
                    var door = hit.collider.GetComponent<DoorController>();
                    if (door != null)
                        door.OpenDoorFromPlayer();
                }
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
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
        }
        else
        {
            if (interactAction.action.WasPressedThisFrame())
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsDoor))
                {
                    var door = hit.collider.GetComponent<DoorController>();
                    if (door != null)
                        door.OpenDoorFromPlayer();

                    return;
                }
            }

            if (heldPickup != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                    {
                        if (hit.transform.GetComponent<ShelfSpaceController>() != null)
                        {
                            hit.transform.GetComponent<ShelfSpaceController>().PlaceStock(heldPickup);
                            if (heldPickup.GetIsPlaced())
                            {
                                heldPickup = null;
                            }
                        }

                    }

                    if (Physics.Raycast(ray, out hit, interactionRange, whatIsTrash))
                    {
                        Destroy(heldPickup.gameObject);
                        heldPickup = null;
                    }
                }

                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    heldPickup.Release();
                    if(heldPickup.gameObject.tag != "PaintCan")
                    {
                        heldPickup.GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
                    }
                    

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

                if (interactAction.action.WasPressedThisFrame())
                {
                    heldBox.OpenClose();
                }
            }

            if (heldFurniture != null)
            {
                heldFurniture.transform.position = new Vector3(furniturePoint.position.x, 0f, furniturePoint.position.z);
                heldFurniture.transform.LookAt(new Vector3(transform.position.x, 0f, transform.position.z));

                if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame)
                {
                    heldFurniture.transform.SetParent(null);

                    heldFurniture.PlaceFurniture();

                    NavMeshcontroller.instance.RebuildNavMesh();

                    heldFurniture = null;
                }
            }

            if(mopObj != null && !mopClean)
            {
                mopObj.transform.position = new Vector3(mopHand.position.x, 0f, mopHand.position.z);
                mopObj.transform.LookAt(new Vector3(transform.position.x, 0f, transform.position.z));
                if(Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (Physics.Raycast(ray, out hit, interactionRange))
                    {
                        if(hit.transform.tag == whatIsMopActionTag)
                        {
                            mopObj.SetParent(null);
                            mopTrashElement = hit.transform.gameObject;
                            mopObj.transform.position = hit.transform.position;
                            mopObj.transform.rotation = Quaternion.identity;
                            mopClean = true;
                            mopObj.GetComponentInChildren<Animator>().SetTrigger("Cleanning");
                        }
                    }
                }
            }

            if(broomObj != null && !broomClean)
            {
                broomObj.transform.position = new Vector3(broomHand.position.x, 0f, broomHand.position.z);
                broomObj.transform.LookAt(new Vector3(transform.position.x, 0f, transform.position.z));
                if(Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (Physics.Raycast(ray, out hit, interactionRange))
                    {
                        if(hit.transform.tag == whatIsBroomActionTag)
                        {
                            broomObj.SetParent(null);
                            broomTrashElement = hit.transform.gameObject;
                            broomObj.transform.position = hit.transform.position;
                            broomObj.transform.rotation = Quaternion.identity;
                            broomClean = true;
                            broomObj.GetComponentInChildren<Animator>().SetTrigger("Cleanning");
                        }
                    }
                }
            }

            if(brushObj != null)
            {
                PaintBrush brush = brushObj.GetComponent<PaintBrush>();
                if(Mouse.current.leftButton.wasPressedThisFrame)
                {
                     if (Physics.Raycast(ray, out hit, interactionRange, whatIsEnvironment))
                    {
                        if (brush.HasPaint()) 
                        {
                            WorldCustomElement custom = hit.transform.GetComponent<WorldCustomElement>();

                                
                            int face = -1;

                            if(hit.transform.GetComponentInParent<WorldCustomElement>())
                            {
                                custom = hit.transform.GetComponentInParent<WorldCustomElement>();
                                if(hit.transform.CompareTag("WallFaceA"))
                                {
                                    face = 0;
                                } else if (hit.transform.CompareTag("WallFaceB"))
                                {
                                    face = 1;
                                } else if(hit.transform.CompareTag("WallFaceC"))
                                {
                                    face = 2;
                                }
                            }

                            if (custom != null && custom.elementType != ElementType.ShopWindow)
                            {
                                custom.PaintElement(brushObj.GetComponent<PaintBrush>().brushPaintMat, face);
                                brush.RemovePaintOnBrush(); 
                            }
                        }
                    }

                    if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                    {
                        if (hit.transform.CompareTag("PaintCan"))
                        {
                            PaintCan paintCan = hit.transform.GetComponent<PaintCan>();

                            if (paintCan != null && paintCan.UsePaintCan())
                            {
                                brush.AddPaintOnBrush(paintCan.matPaint);
                            }
                        }
                    }
                    
                }
            }
        }
    }

    public void CleanMop()
    {
        mopObj.transform.position = Vector3.zero;
        mopObj.transform.rotation = Quaternion.identity;
        mopObj.SetParent(mopHand);
        mopClean = false;
        Destroy(mopTrashElement);
    }

    public void CleanBroom()
    {
        broomObj.transform.position = Vector3.zero;
        broomObj.transform.rotation = Quaternion.identity;
        broomObj.SetParent(broomHand);
        broomClean = false;
        Destroy(broomTrashElement);
    }

    public void RemoveTools()
    {
        if(mopObj != null)
        {
            Destroy(mopObj.gameObject);
            mopClean = false;
            mopObj = null;
        }
        if(broomObj != null)
        {
            Destroy(broomObj.gameObject);
            broomClean = false;
            broomObj = null;
        }
        if(brushObj != null)
        {
            Destroy(brushObj.gameObject);
            brushObj = null;
        }

    }
    
}
