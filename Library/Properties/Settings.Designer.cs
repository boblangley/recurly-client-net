﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Recurly.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}/add_ons")]
        public string PathPlanAddonsList {
            get {
                return ((string)(this["PathPlanAddonsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}/add_ons/{1}")]
        public string PathPlanAddonGet {
            get {
                return ((string)(this["PathPlanAddonGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}/add_ons")]
        public string PathPlanAddonCRUD {
            get {
                return ((string)(this["PathPlanAddonCRUD"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions")]
        public string PathSubscriptionsList {
            get {
                return ((string)(this["PathSubscriptionsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/subscriptions")]
        public string PathAccountSubscriptionsList {
            get {
                return ((string)(this["PathAccountSubscriptionsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}")]
        public string PathSubscriptionGet {
            get {
                return ((string)(this["PathSubscriptionGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions")]
        public string PathSubscriptionCreate {
            get {
                return ((string)(this["PathSubscriptionCreate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}")]
        public string PathSubscriptionUpdate {
            get {
                return ((string)(this["PathSubscriptionUpdate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}/cancel")]
        public string PathSubscriptionCancel {
            get {
                return ((string)(this["PathSubscriptionCancel"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}/reactivate")]
        public string PathSubscriptionReactivate {
            get {
                return ((string)(this["PathSubscriptionReactivate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}/postpone?next_renewal_date={1}")]
        public string PathSubscriptionPostpone {
            get {
                return ((string)(this["PathSubscriptionPostpone"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/subscriptions/{0}/terminate?refund={1}")]
        public string PathSubscriptionTerminate {
            get {
                return ((string)(this["PathSubscriptionTerminate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/invoices")]
        public string PathInvoicesList {
            get {
                return ((string)(this["PathInvoicesList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/invoices")]
        public string PathAccountInvoicesList {
            get {
                return ((string)(this["PathAccountInvoicesList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans")]
        public string PathPlansList {
            get {
                return ((string)(this["PathPlansList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts")]
        public string PathAccountsList {
            get {
                return ((string)(this["PathAccountsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts")]
        public string PathAccountCreate {
            get {
                return ((string)(this["PathAccountCreate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}")]
        public string PathAccountUpdate {
            get {
                return ((string)(this["PathAccountUpdate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}")]
        public string PathAccountClose {
            get {
                return ((string)(this["PathAccountClose"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/reopen")]
        public string PathAccountReopen {
            get {
                return ((string)(this["PathAccountReopen"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/notes")]
        public string PathAccountNotesList {
            get {
                return ((string)(this["PathAccountNotesList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/adjustments")]
        public string PathAccountAdjustmentsList {
            get {
                return ((string)(this["PathAccountAdjustmentsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/adjustments")]
        public string PathAccountAdjustmentCreate {
            get {
                return ((string)(this["PathAccountAdjustmentCreate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/adjustments/{0}")]
        public string PathAccountAdjustmentDelete {
            get {
                return ((string)(this["PathAccountAdjustmentDelete"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/billing_info")]
        public string PathAccountBillingInfoGet {
            get {
                return ((string)(this["PathAccountBillingInfoGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/coupons")]
        public string PathCouponsList {
            get {
                return ((string)(this["PathCouponsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/coupons/{0}")]
        public string PathCouponGet {
            get {
                return ((string)(this["PathCouponGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/coupons")]
        public string PathCouponCreate {
            get {
                return ((string)(this["PathCouponCreate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/coupons/{0}")]
        public string PathCouponDeactivate {
            get {
                return ((string)(this["PathCouponDeactivate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/coupons/{0}/redeem")]
        public string PathRedeemCoupon {
            get {
                return ((string)(this["PathRedeemCoupon"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/invoices/{0}/redemption")]
        public string PathInvoiceCouponRedemption {
            get {
                return ((string)(this["PathInvoiceCouponRedemption"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/redemption")]
        public string PathAccountCouponRedemption {
            get {
                return ((string)(this["PathAccountCouponRedemption"]));
            }
            set {
                this["PathAccountCouponRedemption"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/invoices/{0}")]
        public string PathInvoiceGet {
            get {
                return ((string)(this["PathInvoiceGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/accounts/{0}/invoices")]
        public string PathAccountInvoicePendingCharges {
            get {
                return ((string)(this["PathAccountInvoicePendingCharges"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/invoices/{0}/mark_successful")]
        public string PathInvoiceMarkSuccessful {
            get {
                return ((string)(this["PathInvoiceMarkSuccessful"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/invoices/{0}/mark_failed")]
        public string PathInvoiceMarkFailed {
            get {
                return ((string)(this["PathInvoiceMarkFailed"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}")]
        public string PathPlanGet {
            get {
                return ((string)(this["PathPlanGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans")]
        public string PathPlanCreate {
            get {
                return ((string)(this["PathPlanCreate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}")]
        public string PathPlanUpdate {
            get {
                return ((string)(this["PathPlanUpdate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/plans/{0}")]
        public string PathPlanDelete {
            get {
                return ((string)(this["PathPlanDelete"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/transactions/{0}")]
        public string PathTransactionGet {
            get {
                return ((string)(this["PathTransactionGet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/transactions/{0}")]
        public string PathTransactionFullRefund {
            get {
                return ((string)(this["PathTransactionFullRefund"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/transactions/{0}?amount_in_cents={1}")]
        public string PathTransactionPartialRefund {
            get {
                return ((string)(this["PathTransactionPartialRefund"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/transactions")]
        public string PathTransactionsList {
            get {
                return ((string)(this["PathTransactionsList"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/account/{0}/transactions")]
        public string PathAccountTransactionsList {
            get {
                return ((string)(this["PathAccountTransactionsList"]));
            }
        }
    }
}