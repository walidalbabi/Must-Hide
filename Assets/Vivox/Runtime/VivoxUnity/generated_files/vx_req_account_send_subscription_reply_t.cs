//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class vx_req_account_send_subscription_reply_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal vx_req_account_send_subscription_reply_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(vx_req_account_send_subscription_reply_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~vx_req_account_send_subscription_reply_t() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          VivoxCoreInstancePINVOKE.delete_vx_req_account_send_subscription_reply_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

        public static implicit operator vx_req_base_t(vx_req_account_send_subscription_reply_t t) {
            return t.base_;
        }
        
  public vx_req_base_t base_ {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_base__set(swigCPtr, vx_req_base_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_base__get(swigCPtr);
      vx_req_base_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_req_base_t(cPtr, false);
      return ret;
    } 
  }

  public string account_handle {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_account_handle_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_account_handle_get(swigCPtr);
      return ret;
    } 
  }

  public vx_rule_type rule_type {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_rule_type_set(swigCPtr, (int)value);
    } 
    get {
      vx_rule_type ret = (vx_rule_type)VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_rule_type_get(swigCPtr);
      return ret;
    } 
  }

  public int auto_accept {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_auto_accept_set(swigCPtr, value);
    } 
    get {
      int ret = VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_auto_accept_get(swigCPtr);
      return ret;
    } 
  }

  public string buddy_uri {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_buddy_uri_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_buddy_uri_get(swigCPtr);
      return ret;
    } 
  }

  public string subscription_handle {
    set {
      VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_subscription_handle_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_account_send_subscription_reply_t_subscription_handle_get(swigCPtr);
      return ret;
    } 
  }

  public vx_req_account_send_subscription_reply_t() : this(VivoxCoreInstancePINVOKE.new_vx_req_account_send_subscription_reply_t(), true) {
  }

}
