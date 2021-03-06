﻿using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Aurora.Core.Actions
{
    public class DefaultBindingService : IBindingService
    {
        private readonly Dictionary<KeyStroke, IAction> keyStrokeToActionIdMap = new Dictionary<KeyStroke, IAction>();
        private readonly IActionHandlerService actionHandlerService;

        public DefaultBindingService(IActionHandlerService actionHandlerService)
        {
            this.actionHandlerService = actionHandlerService;
            new KeyDownListener(Application.Current, Dispatch);
        }

        private void Dispatch(KeyEventArgs evt)
        {
            if (IsRejectedKey(evt))
            {
                return;
            }

            var keyStroke = new KeyStroke(evt);

            IAction action;
            if (!keyStrokeToActionIdMap.TryGetValue(keyStroke, out action))
            {
                return;
            }

            var ctx = CreateEventContext();
            var handled = GetActionHandler(ctx)?.Execute(new ActionEvent(action, ctx));
            if(handled.GetValueOrDefault())
            {
                evt.Handled = true;
            }
        }

        private IActionHandler GetActionHandler(IEventContext ctx)
        {
            var depObj = ctx.ActiveElement as DependencyObject;
            if (depObj == null)
            {
                // No active selected element
                return actionHandlerService;
            }
                 
            var i = 0;
            // Recursively find actionHandler along element tree upward
            while (depObj != null)
            {
                var handler = ViewPropertyHelper.GetActionHandlerService(depObj);
                Debug.WriteLine($"DefBindingService - find parent element: {i}: {depObj}:actionHandler={handler}");
                if (handler != null)
                {
                    // Remove actionHandler found from selected element
                    return handler;
                }

                depObj = LogicalTreeHelper.GetParent(depObj);
                i++;
            }

            return actionHandlerService;
        }

        private static bool IsRejectedKey(KeyEventArgs evt)
        {
            switch (evt.RealKey())
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                    return true;
                default:
                    return false;
            }
        }

        private static IEventContext CreateEventContext()
        {
            return new DefaultEventContext(
                Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive),
                Keyboard.FocusedElement,
                Keyboard.FocusedElement
                );
        }

        public void RegisterBinding(KeyStroke keyStroke, IAction action)
        {
            keyStrokeToActionIdMap.Add(keyStroke, action);
            Debug.WriteLine($"Registered binding[{keyStroke}] for action[{action.Id}] - {keyStrokeToActionIdMap.Count} bindings");
        }

        public void UnregisterBinding(KeyStroke keyStroke)
        {
            keyStrokeToActionIdMap.Remove(keyStroke);
            Debug.WriteLine($"Unregistered binding[{keyStroke}] - {keyStrokeToActionIdMap.Count} bindings");
        }

        public IDictionary<KeyStroke, IAction> GetAllBindings()
        {
            return new ReadOnlyDictionary<KeyStroke, IAction>(this.keyStrokeToActionIdMap);
        }
    }
}
