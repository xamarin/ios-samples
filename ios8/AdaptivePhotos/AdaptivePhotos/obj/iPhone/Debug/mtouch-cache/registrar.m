#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#define DEBUG 1
#include <stdarg.h>
#include <monotouch/monotouch.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


bool native_to_managed_trampoline_1 (id self, SEL _cmd, MonoMethod **managed_method_ptr, void * p0, const char *r0, const char *r1, const char *r2)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [1];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[1] = { r0 };
		managed_method = get_method_direct(r1, r2, 1, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	void * a0 = p0;
	arg_ptrs [0] = &a0;

	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	bool res;
	res = *(bool *) mono_object_unbox (retval);

	return res;
}


id native_to_managed_trampoline_2 (id self, SEL _cmd, MonoMethod **managed_method_ptr, const char *r0, const char *r1)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [0];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	if (monotouch_try_get_nsobject (self))
		return self;
	if (!managed_method) {
		managed_method = get_method_direct(r0, r1, 0, NULL)->method;
		*managed_method_ptr = managed_method;
	}
	mthis = mono_object_new (mono_domain_get (), mono_method_get_class (managed_method));
	uint8_t flags = 2;
	mono_field_set_value (mthis, monotouch_nsobject_handle_field, &self);
	mono_field_set_value (mthis, monotouch_nsobject_flags_field, &flags);
	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);
	monotouch_create_managed_ref (self, mthis, true);

	return self;
}


void native_to_managed_trampoline_3 (id self, SEL _cmd, MonoMethod **managed_method_ptr, const char *r0, const char *r1)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [0];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		managed_method = get_method_direct(r0, r1, 0, NULL)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


void native_to_managed_trampoline_4 (id self, SEL _cmd, MonoMethod **managed_method_ptr, bool p0, const char *r0, const char *r1, const char *r2)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [1];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[1] = { r0 };
		managed_method = get_method_direct(r1, r2, 1, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	arg_ptrs [0] = &p0;

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


void native_to_managed_trampoline_5 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, const char *r0, const char *r1, const char *r2)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [1];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[1] = { r0 };
		managed_method = get_method_direct(r1, r2, 1, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


int native_to_managed_trampoline_6 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, int p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	arg_ptrs [1] = &p1;

	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	int res;
	res = *(int *) mono_object_unbox (retval);

	return res;
}


id native_to_managed_trampoline_7 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	NSObject *nsobj1 = (NSObject *) p1;
	MonoObject *mobj1 = NULL;
	bool created1 = false;
	if (nsobj1) {
		MonoType *paramtype1 = monotouch_get_parameter_type (managed_method, 1);
		mobj1 = get_nsobject_with_type_for_ptr_created (nsobj1, false, paramtype1, &created1);
		monotouch_verify_parameter (mobj1, _cmd, self, nsobj1, 1, mono_class_from_mono_type (paramtype1), managed_method);
	}
	arg_ptrs [1] = mobj1;

	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	id res;
	if (!retval) {
		res = NULL;
	} else {
		id retobj;
		mono_field_get_value ((MonoObject *) retval, monotouch_nsobject_handle_field, (void **) &retobj);
		monotouch_framework_peer_lock ();
		[retobj retain];
		monotouch_framework_peer_unlock ();
		[retobj autorelease];
		mt_dummy_use (retval);
		res = retobj;
	}

	return res;
}


void native_to_managed_trampoline_8 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, id p2, const char *r0, const char *r1, const char *r2, const char *r3, const char *r4)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [3];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[3] = { r0, r1, r2 };
		managed_method = get_method_direct(r3, r4, 3, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	NSObject *nsobj1 = (NSObject *) p1;
	MonoObject *mobj1 = NULL;
	bool created1 = false;
	if (nsobj1) {
		MonoType *paramtype1 = monotouch_get_parameter_type (managed_method, 1);
		mobj1 = get_nsobject_with_type_for_ptr_created (nsobj1, false, paramtype1, &created1);
		monotouch_verify_parameter (mobj1, _cmd, self, nsobj1, 1, mono_class_from_mono_type (paramtype1), managed_method);
	}
	arg_ptrs [1] = mobj1;
	NSObject *nsobj2 = (NSObject *) p2;
	MonoObject *mobj2 = NULL;
	bool created2 = false;
	if (nsobj2) {
		MonoType *paramtype2 = monotouch_get_parameter_type (managed_method, 2);
		mobj2 = get_nsobject_with_type_for_ptr_created (nsobj2, false, paramtype2, &created2);
		monotouch_verify_parameter (mobj2, _cmd, self, nsobj2, 2, mono_class_from_mono_type (paramtype2), managed_method);
	}
	arg_ptrs [2] = mobj2;

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


void native_to_managed_trampoline_9 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	NSObject *nsobj1 = (NSObject *) p1;
	MonoObject *mobj1 = NULL;
	bool created1 = false;
	if (nsobj1) {
		MonoType *paramtype1 = monotouch_get_parameter_type (managed_method, 1);
		mobj1 = get_nsobject_with_type_for_ptr_created (nsobj1, false, paramtype1, &created1);
		monotouch_verify_parameter (mobj1, _cmd, self, nsobj1, 1, mono_class_from_mono_type (paramtype1), managed_method);
	}
	arg_ptrs [1] = mobj1;

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


bool native_to_managed_trampoline_10 (id self, SEL _cmd, MonoMethod **managed_method_ptr, const char *r0, const char *r1)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [0];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		managed_method = get_method_direct(r0, r1, 0, NULL)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	bool res;
	res = *(bool *) mono_object_unbox (retval);

	return res;
}


CGSize native_to_managed_trampoline_11 (id self, SEL _cmd, MonoMethod **managed_method_ptr, const char *r0, const char *r1)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [0];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		managed_method = get_method_direct(r0, r1, 0, NULL)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	CGSize res;
	res = *(CGSize *) mono_object_unbox (retval);

	return res;
}


void native_to_managed_trampoline_12 (id self, SEL _cmd, MonoMethod **managed_method_ptr, CGSize p0, id p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	arg_ptrs [0] = &p0;
	arg_ptrs [1] = monotouch_get_inative_object_static (p1, false, "MonoTouch.UIKit.UIViewControllerTransitionCoordinatorWrapper, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.IUIViewControllerTransitionCoordinator, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065");

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


bool native_to_managed_trampoline_13 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, id p2, const char *r0, const char *r1, const char *r2, const char *r3, const char *r4)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [3];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[3] = { r0, r1, r2 };
		managed_method = get_method_direct(r3, r4, 3, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	NSObject *nsobj1 = (NSObject *) p1;
	MonoObject *mobj1 = NULL;
	bool created1 = false;
	if (nsobj1) {
		MonoType *paramtype1 = monotouch_get_parameter_type (managed_method, 1);
		mobj1 = get_nsobject_with_type_for_ptr_created (nsobj1, false, paramtype1, &created1);
		monotouch_verify_parameter (mobj1, _cmd, self, nsobj1, 1, mono_class_from_mono_type (paramtype1), managed_method);
	}
	arg_ptrs [1] = mobj1;
	NSObject *nsobj2 = (NSObject *) p2;
	MonoObject *mobj2 = NULL;
	bool created2 = false;
	if (nsobj2) {
		MonoType *paramtype2 = monotouch_get_parameter_type (managed_method, 2);
		mobj2 = get_nsobject_with_type_for_ptr_created (nsobj2, false, paramtype2, &created2);
		monotouch_verify_parameter (mobj2, _cmd, self, nsobj2, 2, mono_class_from_mono_type (paramtype2), managed_method);
	}
	arg_ptrs [2] = mobj2;

	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	bool res;
	res = *(bool *) mono_object_unbox (retval);

	return res;
}


bool native_to_managed_trampoline_14 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	NSObject *nsobj1 = (NSObject *) p1;
	MonoObject *mobj1 = NULL;
	bool created1 = false;
	if (nsobj1) {
		MonoType *paramtype1 = monotouch_get_parameter_type (managed_method, 1);
		mobj1 = get_nsobject_with_type_for_ptr_created (nsobj1, false, paramtype1, &created1);
		monotouch_verify_parameter (mobj1, _cmd, self, nsobj1, 1, mono_class_from_mono_type (paramtype1), managed_method);
	}
	arg_ptrs [1] = mobj1;

	void * retval = mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	bool res;
	res = *(bool *) mono_object_unbox (retval);

	return res;
}


void native_to_managed_trampoline_15 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, id p1, const char *r0, const char *r1, const char *r2, const char *r3)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [2];
	MonoObject *mthis;
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	mthis = NULL;
	if (self) {
		mthis = get_managed_object_for_ptr_fast (self, false);
	}
	if (!managed_method) {
		const char *paramptr[2] = { r0, r1 };
		managed_method = get_method_direct(r2, r3, 2, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	check_for_gced_object (mthis, _cmd, self, managed_method);
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;
	arg_ptrs [1] = monotouch_get_inative_object_static (p1, false, "MonoTouch.UIKit.UIViewControllerTransitionCoordinatorWrapper, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.IUIViewControllerTransitionCoordinator, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065");

	mono_runtime_invoke (managed_method, mthis, arg_ptrs, NULL);

	return;
}


void native_to_managed_trampoline_16 (id self, SEL _cmd, MonoMethod **managed_method_ptr, id p0, const char *r0, const char *r1, const char *r2)
{
	MonoMethod *managed_method = *managed_method_ptr;
	void *arg_ptrs [1];
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	if (!managed_method) {
		const char *paramptr[1] = { r0 };
		managed_method = get_method_direct(r1, r2, 1, paramptr)->method;
		*managed_method_ptr = managed_method;
	}
	NSObject *nsobj0 = (NSObject *) p0;
	MonoObject *mobj0 = NULL;
	bool created0 = false;
	if (nsobj0) {
		MonoType *paramtype0 = monotouch_get_parameter_type (managed_method, 0);
		mobj0 = get_nsobject_with_type_for_ptr_created (nsobj0, false, paramtype0, &created0);
		monotouch_verify_parameter (mobj0, _cmd, self, nsobj0, 0, mono_class_from_mono_type (paramtype0), managed_method);
	}
	arg_ptrs [0] = mobj0;

	mono_runtime_invoke (managed_method, NULL, arg_ptrs, NULL);

	return;
}



@interface AdaptivePhotos_AAPLPhoto : NSObject {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLPhoto { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLPhoto, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLConversation : NSObject {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLConversation { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLConversation, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_CustomTableViewController : UITableViewController {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) conformsToProtocol:(void *)p0;
@end
@implementation AdaptivePhotos_CustomTableViewController { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}
@end

@interface AdaptivePhotos_AAPLListTableViewController : AdaptivePhotos_CustomTableViewController {
}
	-(void) viewDidLoad;
	-(void) viewWillAppear:(bool)p0;
	-(void) showDetailTargetDidChange:(id)p0;
	-(int) tableView:(id)p0 numberOfRowsInSection:(int)p1;
	-(id) tableView:(id)p0 cellForRowAtIndexPath:(id)p1;
	-(void) tableView:(id)p0 willDisplayCell:(id)p1 forRowAtIndexPath:(id)p2;
	-(void) tableView:(id)p0 didSelectRowAtIndexPath:(id)p1;
@end
@implementation AdaptivePhotos_AAPLListTableViewController { } 

	-(void) viewDidLoad
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewDidLoad");
	}

	-(void) viewWillAppear:(bool)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_4 (self, _cmd, &managed_method, p0, "System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewWillAppear");
	}

	-(void) showDetailTargetDidChange:(id)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_5 (self, _cmd, &managed_method, p0, "MonoTouch.Foundation.NSNotification, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ShowDetailTargetDidChange");
	}

	-(int) tableView:(id)p0 numberOfRowsInSection:(int)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_6 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "RowsInSection");
	}

	-(id) tableView:(id)p0 cellForRowAtIndexPath:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_7 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "GetCell");
	}

	-(void) tableView:(id)p0 willDisplayCell:(id)p1 forRowAtIndexPath:(id)p2
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_8 (self, _cmd, &managed_method, p0, p1, p2, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UITableViewCell, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "WillDisplay");
	}

	-(void) tableView:(id)p0 didSelectRowAtIndexPath:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_9 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "RowSelected");
	}
@end

@interface AdaptivePhotos_CustomViewController : UIViewController {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_CustomViewController { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.CustomViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_CustomNavigationController : UINavigationController {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) Aapl_willShowingViewControllerPushWithSender;
	-(bool) conformsToProtocol:(void *)p0;
@end
@implementation AdaptivePhotos_CustomNavigationController { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) Aapl_willShowingViewControllerPushWithSender
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.CustomNavigationController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Aapl_willShowingDetailViewControllerPushWithSender");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}
@end

@interface AdaptivePhotos_CustomSplitViewController : UISplitViewController {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) Aapl_willShowingViewControllerPushWithSender;
	-(bool) Aapl_willShowingDetailViewControllerPushWithSender;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_CustomSplitViewController { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) Aapl_willShowingViewControllerPushWithSender
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.CustomSplitViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Aapl_willShowingViewControllerPushWithSender");
	}

	-(bool) Aapl_willShowingDetailViewControllerPushWithSender
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.CustomSplitViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Aapl_willShowingDetailViewControllerPushWithSender");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.CustomSplitViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLEmptyViewController : AdaptivePhotos_CustomViewController {
}
	-(void) viewDidLoad;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLEmptyViewController { } 

	-(void) viewDidLoad
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLEmptyViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewDidLoad");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLEmptyViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLOverlayView : UIView {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(CGSize) intrinsicContentSize;
	-(void) traitCollectionDidChange:(id)p0;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLOverlayView { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(CGSize) intrinsicContentSize
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_11 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLOverlayView, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "get_IntrinsicContentSize");
	}

	-(void) traitCollectionDidChange:(id)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_5 (self, _cmd, &managed_method, p0, "MonoTouch.UIKit.UITraitCollection, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLOverlayView, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "TraitCollectionDidChange");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLOverlayView, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLRatingControl : UIControl {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) isAccessibilityElement;
	-(void) setIsAccessibilityElement:(bool)p0;
	-(void) touchesBegan:(id)p0 withEvent:(id)p1;
	-(void) touchesMoved:(id)p0 withEvent:(id)p1;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLRatingControl { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) isAccessibilityElement
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "get_IsAccessibilityElement");
	}

	-(void) setIsAccessibilityElement:(bool)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_4 (self, _cmd, &managed_method, p0, "System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "set_IsAccessibilityElement");
	}

	-(void) touchesBegan:(id)p0 withEvent:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_9 (self, _cmd, &managed_method, p0, p1, "MonoTouch.Foundation.NSSet, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIEvent, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "TouchesBegan");
	}

	-(void) touchesMoved:(id)p0 withEvent:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_9 (self, _cmd, &managed_method, p0, p1, "MonoTouch.Foundation.NSSet, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIEvent, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "TouchesMoved");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLTraitOverrideViewController : AdaptivePhotos_CustomViewController {
}
	-(bool) shouldAutomaticallyForwardAppearanceMethods;
	-(bool) shouldAutomaticallyForwardRotationMethods;
	-(void) viewWillTransitionToSize:(CGSize)p0 withTransitionCoordinator:(id)p1;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLTraitOverrideViewController { } 

	-(bool) shouldAutomaticallyForwardAppearanceMethods
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLTraitOverrideViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "get_ShouldAutomaticallyForwardAppearanceMethods");
	}

	-(bool) shouldAutomaticallyForwardRotationMethods
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_10 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLTraitOverrideViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "get_ShouldAutomaticallyForwardRotationMethods");
	}

	-(void) viewWillTransitionToSize:(CGSize)p0 withTransitionCoordinator:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_12 (self, _cmd, &managed_method, p0, p1, "System.Drawing.SizeF, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.IUIViewControllerTransitionCoordinator, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLTraitOverrideViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewWillTransitionToSize");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLTraitOverrideViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLConversationViewController : AdaptivePhotos_CustomTableViewController {
}
	-(void) viewDidLoad;
	-(void) viewWillAppear:(bool)p0;
	-(void) showDetailTargetDidChange:(id)p0;
	-(int) tableView:(id)p0 numberOfRowsInSection:(int)p1;
	-(id) tableView:(id)p0 cellForRowAtIndexPath:(id)p1;
	-(void) tableView:(id)p0 willDisplayCell:(id)p1 forRowAtIndexPath:(id)p2;
	-(void) tableView:(id)p0 didSelectRowAtIndexPath:(id)p1;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLConversationViewController { } 

	-(void) viewDidLoad
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewDidLoad");
	}

	-(void) viewWillAppear:(bool)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_4 (self, _cmd, &managed_method, p0, "System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ViewWillAppear");
	}

	-(void) showDetailTargetDidChange:(id)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_5 (self, _cmd, &managed_method, p0, "MonoTouch.Foundation.NSNotification, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "ShowDetailTargetDidChange");
	}

	-(int) tableView:(id)p0 numberOfRowsInSection:(int)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_6 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "RowsInSection");
	}

	-(id) tableView:(id)p0 cellForRowAtIndexPath:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_7 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "GetCell");
	}

	-(void) tableView:(id)p0 willDisplayCell:(id)p1 forRowAtIndexPath:(id)p2
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_8 (self, _cmd, &managed_method, p0, p1, p2, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UITableViewCell, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "WillDisplay");
	}

	-(void) tableView:(id)p0 didSelectRowAtIndexPath:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_9 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "RowSelected");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLPhotoViewController : AdaptivePhotos_CustomViewController {
}
	-(void) loadView;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLPhotoViewController { } 

	-(void) loadView
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLPhotoViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "LoadView");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLPhotoViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AppDelegate_SplitViewControllerDelegate : NSObject/*<UISplitViewControllerDelegate>*/ {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) splitViewController:(id)p0 collapseSecondaryViewController:(id)p1 ontoPrimaryViewController:(id)p2;
	-(id) splitViewController:(id)p0 separateSecondaryViewControllerFromPrimaryViewController:(id)p1;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AdaptivePhotos_AppDelegate_SplitViewControllerDelegate { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) splitViewController:(id)p0 collapseSecondaryViewController:(id)p1 ontoPrimaryViewController:(id)p2
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_13 (self, _cmd, &managed_method, p0, p1, p2, "MonoTouch.UIKit.UISplitViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AppDelegate+SplitViewControllerDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "CollapseSecondViewController");
	}

	-(id) splitViewController:(id)p0 separateSecondaryViewControllerFromPrimaryViewController:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_7 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UISplitViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AppDelegate+SplitViewControllerDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "SeparateSecondaryViewController");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AppDelegate+SplitViewControllerDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AppDelegate : NSObject/*<UIApplicationDelegate>*/ {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(bool) application:(id)p0 didFinishLaunchingWithOptions:(id)p1;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation AppDelegate { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(bool) application:(id)p0 didFinishLaunchingWithOptions:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_14 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UIApplication, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSDictionary, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AppDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "FinishedLaunching");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AppDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface AdaptivePhotos_AAPLProfileViewController : AdaptivePhotos_CustomViewController {
}
	-(void) loadView;
	-(void) willTransitionToTraitCollection:(id)p0 withTransitionCoordinator:(id)p1;
	-(id) init;
@end
@implementation AdaptivePhotos_AAPLProfileViewController { } 

	-(void) loadView
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLProfileViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "LoadView");
	}

	-(void) willTransitionToTraitCollection:(id)p0 withTransitionCoordinator:(id)p1
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_15 (self, _cmd, &managed_method, p0, p1, "MonoTouch.UIKit.UITraitCollection, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.IUIViewControllerTransitionCoordinator, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "AdaptivePhotos.AAPLProfileViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "WillTransitionToTraitCollection");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "AdaptivePhotos.AAPLProfileViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ".ctor");
	}
@end

@interface MonoTouch_UIKit_UIControlEventProxy : NSObject {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(void) BridgeSelector;
	-(bool) conformsToProtocol:(void *)p0;
@end
@implementation MonoTouch_UIKit_UIControlEventProxy { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(void) BridgeSelector
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_3 (self, _cmd, &managed_method, "MonoTouch.UIKit.UIControlEventProxy, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "Activated");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}
@end

@interface __NSObject_Disposer : NSObject {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	+(void) drain:(id)p0;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation __NSObject_Disposer { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	+(void) drain:(id)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_16 (self, _cmd, &managed_method, p0, "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.Foundation.NSObject+NSObject_Disposer, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "Drain");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "MonoTouch.Foundation.NSObject+NSObject_Disposer, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", ".ctor");
	}
@end

@interface MonoTouch_UIKit_UIBarButtonItem_Callback : NSObject {
	void *__monoObjectGCHandle;
}
	-(void) release;
	-(id) retain;
	-(void) dealloc;
	-(void) InvokeAction:(id)p0;
	-(bool) conformsToProtocol:(void *)p0;
	-(id) init;
@end
@implementation MonoTouch_UIKit_UIBarButtonItem_Callback { } 
	-(void) release
	{
		monotouch_release_trampoline (self, _cmd);
	}

	-(id) retain
	{
		return monotouch_retain_trampoline (self, _cmd);
	}

	-(void) dealloc
	{
		int gchandle = monotouch_get_gchandle (self);
		monotouch_unregister_object (self, mono_gchandle_get_target (gchandle));
		monotouch_free_gchandle (self, gchandle);
		[super dealloc];
	}

	-(void) InvokeAction:(id)p0
	{
		static MonoMethod *managed_method = NULL;
		native_to_managed_trampoline_5 (self, _cmd, &managed_method, p0, "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "MonoTouch.UIKit.UIBarButtonItem+Callback, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "Call");
	}

	-(bool) conformsToProtocol:(void *)p0
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_1 (self, _cmd, &managed_method, p0, "System.IntPtr, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", "InvokeConformsToProtocol");
	}

	-(id) init
	{
		static MonoMethod *managed_method = NULL;
		return native_to_managed_trampoline_2 (self, _cmd, &managed_method, "MonoTouch.UIKit.UIBarButtonItem+Callback, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", ".ctor");
	}
@end

	static MTClassMap __monotouch_class_map [] = {
		{"NSObject", "MonoTouch.Foundation.NSObject, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_AAPLPhoto", "AdaptivePhotos.AAPLPhoto, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLConversation", "AdaptivePhotos.AAPLConversation, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"UIResponder", "MonoTouch.UIKit.UIResponder, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIViewController", "MonoTouch.UIKit.UIViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UITableViewController", "MonoTouch.UIKit.UITableViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_CustomTableViewController", "AdaptivePhotos.CustomTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLListTableViewController", "AdaptivePhotos.AAPLListTableViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_CustomViewController", "AdaptivePhotos.CustomViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"UINavigationController", "MonoTouch.UIKit.UINavigationController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_CustomNavigationController", "AdaptivePhotos.CustomNavigationController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"UISplitViewController", "MonoTouch.UIKit.UISplitViewController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_CustomSplitViewController", "AdaptivePhotos.CustomSplitViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLEmptyViewController", "AdaptivePhotos.AAPLEmptyViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"UIView", "MonoTouch.UIKit.UIView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_AAPLOverlayView", "AdaptivePhotos.AAPLOverlayView, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"UIControl", "MonoTouch.UIKit.UIControl, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"AdaptivePhotos_AAPLRatingControl", "AdaptivePhotos.AAPLRatingControl, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLTraitOverrideViewController", "AdaptivePhotos.AAPLTraitOverrideViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLConversationViewController", "AdaptivePhotos.AAPLConversationViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLPhotoViewController", "AdaptivePhotos.AAPLPhotoViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AppDelegate_SplitViewControllerDelegate", "AdaptivePhotos.AppDelegate+SplitViewControllerDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AppDelegate", "AdaptivePhotos.AppDelegate, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"AdaptivePhotos_AAPLProfileViewController", "AdaptivePhotos.AAPLProfileViewController, AdaptivePhotos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", NULL },
		{"NSArray", "MonoTouch.Foundation.NSArray, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSBundle", "MonoTouch.Foundation.NSBundle, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSIndexPath", "MonoTouch.Foundation.NSIndexPath, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSLayoutConstraint", "MonoTouch.UIKit.NSLayoutConstraint, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSValue", "MonoTouch.Foundation.NSValue, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSNumber", "MonoTouch.Foundation.NSNumber, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSString", "MonoTouch.Foundation.NSString, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSAutoreleasePool", "MonoTouch.Foundation.NSAutoreleasePool, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIApplication", "MonoTouch.UIKit.UIApplication, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIBarItem", "MonoTouch.UIKit.UIBarItem, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIColor", "MonoTouch.UIKit.UIColor, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"MonoTouch_UIKit_UIControlEventProxy", "MonoTouch.UIKit.UIControlEventProxy, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIDevice", "MonoTouch.UIKit.UIDevice, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIFont", "MonoTouch.UIKit.UIFont, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIEvent", "MonoTouch.UIKit.UIEvent, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIImage", "MonoTouch.UIKit.UIImage, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIScreen", "MonoTouch.UIKit.UIScreen, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIScrollView", "MonoTouch.UIKit.UIScrollView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UITableView", "MonoTouch.UIKit.UITableView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UITableViewCell", "MonoTouch.UIKit.UITableViewCell, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIWindow", "MonoTouch.UIKit.UIWindow, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UILabel", "MonoTouch.UIKit.UILabel, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIImageView", "MonoTouch.UIKit.UIImageView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UINavigationItem", "MonoTouch.UIKit.UINavigationItem, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIPresentationController", "MonoTouch.UIKit.UIPresentationController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UITouch", "MonoTouch.UIKit.UITouch, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UITraitCollection", "MonoTouch.UIKit.UITraitCollection, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIPopoverPresentationController", "MonoTouch.UIKit.UIPopoverPresentationController, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIVisualEffect", "MonoTouch.UIKit.UIVisualEffect, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIBlurEffect", "MonoTouch.UIKit.UIBlurEffect, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIVisualEffectView", "MonoTouch.UIKit.UIVisualEffectView, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSMutableArray", "MonoTouch.Foundation.NSMutableArray, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSEnumerator", "MonoTouch.Foundation.NSEnumerator, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSException", "MonoTouch.Foundation.NSException, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSNull", "MonoTouch.Foundation.NSNull, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSNotification", "MonoTouch.Foundation.NSNotification, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSDictionary", "MonoTouch.Foundation.NSDictionary, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSNotificationCenter", "MonoTouch.Foundation.NSNotificationCenter, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"NSSet", "MonoTouch.Foundation.NSSet, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"__NSObject_Disposer", "MonoTouch.Foundation.NSObject+NSObject_Disposer, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"MonoTouch_UIKit_UIBarButtonItem_Callback", "MonoTouch.UIKit.UIBarButtonItem+Callback, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{"UIBarButtonItem", "MonoTouch.UIKit.UIBarButtonItem, monotouch, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", NULL },
		{ NULL, NULL, NULL },
	};


void monotouch_create_classes () {
	__monotouch_class_map [0].handle = objc_getClass ("NSObject");
	__monotouch_class_map [1].handle = [AdaptivePhotos_AAPLPhoto class];
	__monotouch_class_map [2].handle = [AdaptivePhotos_AAPLConversation class];
	__monotouch_class_map [3].handle = objc_getClass ("UIResponder");
	__monotouch_class_map [4].handle = objc_getClass ("UIViewController");
	__monotouch_class_map [5].handle = objc_getClass ("UITableViewController");
	__monotouch_class_map [6].handle = [AdaptivePhotos_CustomTableViewController class];
	__monotouch_class_map [7].handle = [AdaptivePhotos_AAPLListTableViewController class];
	__monotouch_class_map [8].handle = [AdaptivePhotos_CustomViewController class];
	__monotouch_class_map [9].handle = objc_getClass ("UINavigationController");
	__monotouch_class_map [10].handle = [AdaptivePhotos_CustomNavigationController class];
	__monotouch_class_map [11].handle = objc_getClass ("UISplitViewController");
	__monotouch_class_map [12].handle = [AdaptivePhotos_CustomSplitViewController class];
	__monotouch_class_map [13].handle = [AdaptivePhotos_AAPLEmptyViewController class];
	__monotouch_class_map [14].handle = objc_getClass ("UIView");
	__monotouch_class_map [15].handle = [AdaptivePhotos_AAPLOverlayView class];
	__monotouch_class_map [16].handle = objc_getClass ("UIControl");
	__monotouch_class_map [17].handle = [AdaptivePhotos_AAPLRatingControl class];
	__monotouch_class_map [18].handle = [AdaptivePhotos_AAPLTraitOverrideViewController class];
	__monotouch_class_map [19].handle = [AdaptivePhotos_AAPLConversationViewController class];
	__monotouch_class_map [20].handle = [AdaptivePhotos_AAPLPhotoViewController class];
	__monotouch_class_map [21].handle = [AdaptivePhotos_AppDelegate_SplitViewControllerDelegate class];
	__monotouch_class_map [22].handle = [AppDelegate class];
	__monotouch_class_map [23].handle = [AdaptivePhotos_AAPLProfileViewController class];
	__monotouch_class_map [24].handle = objc_getClass ("NSArray");
	__monotouch_class_map [25].handle = objc_getClass ("NSBundle");
	__monotouch_class_map [26].handle = objc_getClass ("NSIndexPath");
	__monotouch_class_map [27].handle = objc_getClass ("NSLayoutConstraint");
	__monotouch_class_map [28].handle = objc_getClass ("NSValue");
	__monotouch_class_map [29].handle = objc_getClass ("NSNumber");
	__monotouch_class_map [30].handle = objc_getClass ("NSString");
	__monotouch_class_map [31].handle = objc_getClass ("NSAutoreleasePool");
	__monotouch_class_map [32].handle = objc_getClass ("UIApplication");
	__monotouch_class_map [33].handle = objc_getClass ("UIBarItem");
	__monotouch_class_map [34].handle = objc_getClass ("UIColor");
	__monotouch_class_map [35].handle = objc_getClass ("MonoTouch_UIKit_UIControlEventProxy");
	__monotouch_class_map [36].handle = objc_getClass ("UIDevice");
	__monotouch_class_map [37].handle = objc_getClass ("UIFont");
	__monotouch_class_map [38].handle = objc_getClass ("UIEvent");
	__monotouch_class_map [39].handle = objc_getClass ("UIImage");
	__monotouch_class_map [40].handle = objc_getClass ("UIScreen");
	__monotouch_class_map [41].handle = objc_getClass ("UIScrollView");
	__monotouch_class_map [42].handle = objc_getClass ("UITableView");
	__monotouch_class_map [43].handle = objc_getClass ("UITableViewCell");
	__monotouch_class_map [44].handle = objc_getClass ("UIWindow");
	__monotouch_class_map [45].handle = objc_getClass ("UILabel");
	__monotouch_class_map [46].handle = objc_getClass ("UIImageView");
	__monotouch_class_map [47].handle = objc_getClass ("UINavigationItem");
	__monotouch_class_map [48].handle = objc_getClass ("UIPresentationController");
	__monotouch_class_map [49].handle = objc_getClass ("UITouch");
	__monotouch_class_map [50].handle = objc_getClass ("UITraitCollection");
	__monotouch_class_map [51].handle = objc_getClass ("UIPopoverPresentationController");
	__monotouch_class_map [52].handle = objc_getClass ("UIVisualEffect");
	__monotouch_class_map [53].handle = objc_getClass ("UIBlurEffect");
	__monotouch_class_map [54].handle = objc_getClass ("UIVisualEffectView");
	__monotouch_class_map [55].handle = objc_getClass ("NSMutableArray");
	__monotouch_class_map [56].handle = objc_getClass ("NSEnumerator");
	__monotouch_class_map [57].handle = objc_getClass ("NSException");
	__monotouch_class_map [58].handle = objc_getClass ("NSNull");
	__monotouch_class_map [59].handle = objc_getClass ("NSNotification");
	__monotouch_class_map [60].handle = objc_getClass ("NSDictionary");
	__monotouch_class_map [61].handle = objc_getClass ("NSNotificationCenter");
	__monotouch_class_map [62].handle = objc_getClass ("NSSet");
	__monotouch_class_map [63].handle = objc_getClass ("__NSObject_Disposer");
	__monotouch_class_map [64].handle = objc_getClass ("MonoTouch_UIKit_UIBarButtonItem_Callback");
	__monotouch_class_map [65].handle = objc_getClass ("UIBarButtonItem");
	monotouch_setup_classmap (__monotouch_class_map, 66);
}

