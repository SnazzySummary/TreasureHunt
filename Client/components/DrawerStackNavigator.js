import "react-native-gesture-handler";
import { createStackNavigator } from "@react-navigation/stack";
import DrawerNavigator from "./DrawerNavigation/DrawerNavigator";
import { TransitionPresets } from "@react-navigation/stack";
import Notifications from "./Notifications/Notifications";

const Stack = createStackNavigator();

function DrawerStackNavigator() {
  return (
    <Stack.Navigator>
      <Stack.Screen
        name="Drawer"
        component={DrawerNavigator}
        options={{ headerShown: false, gestureEnabled: false }}
      />
      <Stack.Screen
        name="Notifications"
        component={Notifications}
        options={{
          headerShown: true,
          gestureEnabled: true,
          ...TransitionPresets.SlideFromRightIOS,
        }}
      />
    </Stack.Navigator>
  );
}

export default DrawerStackNavigator;
