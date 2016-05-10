using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// The Controller Class is simply a set of properties used to separate user intention from the actual user input.
/// This was done so that changing key bindings would be easier and, if we so wished, we could include xbox controls as well.
/// </summary>
public class Controller : MonoBehaviour {

    static public bool Select { get {
            return Input.GetMouseButtonDown(0);
        } }

    static public bool Drag { get {
            return Input.GetMouseButton(0);
        } }

    static public bool Back { get {
            return Input.GetKeyDown(KeyCode.Escape);
        } }

    static public bool Pause { get {
            return Input.GetKeyDown(KeyCode.Escape)
                || Input.GetKeyDown(KeyCode.P);
        } }

    static public float LookHorizontal { get {
            return Input.GetAxis("Mouse X");
        } }

    static public float LookVertical { get {
            return Input.GetAxis("Mouse Y");
        } }

    static public bool Fire { get {
            return Input.GetMouseButtonDown(0);
        } }

    static public bool Interact { get {
            return Input.GetKeyDown(KeyCode.E);
        } }

    static public bool Boots { get {
            return Input.GetMouseButtonDown(1);
        } }

    static public bool MoveForward {  get {
            return Input.GetKey(KeyCode.W);
        } }

    static public bool MoveBackward {  get {
            return Input.GetKey(KeyCode.S);
        } }

    static public bool StrafeLeft {  get {
            return Input.GetKey(KeyCode.A);
        } }

    static public bool StrafeRight {  get {
            return Input.GetKey(KeyCode.D);
        } }

    static public bool Jump {  get {
            return Input.GetKeyDown(KeyCode.Space);
        } }

    static public bool Sprint {  get {
            return Input.GetKey(KeyCode.LeftShift)
                || Input.GetKey(KeyCode.RightShift);
        } }

    static public bool Flashlight { get {
    	return Input.GetKeyDown(KeyCode.F);
    }}
}
