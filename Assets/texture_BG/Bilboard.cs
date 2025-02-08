using UnityEngine;

public class Billboard : MonoBehaviour
{
   void LateUpdate()
{
    Camera cam = Camera.main; 
    if (cam == null) return;
    
    transform.LookAt(cam.transform);
    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
}

}
