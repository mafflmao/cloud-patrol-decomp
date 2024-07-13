using System.Collections.Generic;

public interface IActionButton
{
	List<UIActionInfo> GetActionInfo();

	bool EventMatchWithButton(POINTER_INFO.INPUT_EVENT aEvent);

	void InvokeAllMethodForEvent(POINTER_INFO i_ptr);
}
