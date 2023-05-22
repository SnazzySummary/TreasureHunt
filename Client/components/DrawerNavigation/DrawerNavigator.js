import "react-native-gesture-handler";
import React, { Fragment } from "react";
import { createDrawerNavigator } from "@react-navigation/drawer";

import TreasureHunts from "../TreasureHunt/TreasureHunts";
import Settings from "../Settings";
import Profile from "../Profile";
import About from "../About";
import NotificationIcon from "../Notifications/NotificationIcon";

export default function DrawerNavigator({ navigation }) {
  const Drawer = createDrawerNavigator();

  const headerAccessories = {
    headerRight: () => (
      <NotificationIcon
        navigationAction={() => {
          navigation.navigate("Notifications");
        }}
      />
    ),
  };

  return (
    <Fragment>
      <Drawer.Navigator screenOptions={headerAccessories}>
        <Drawer.Screen name="TreasureHunts" component={TreasureHunts} />
        <Drawer.Screen name="Profile" component={Profile} />
        <Drawer.Screen name="About" component={About} />
        <Drawer.Screen name="Settings" component={Settings} />
        <Drawer.Screen name="Logout" component={Settings} />
      </Drawer.Navigator>
    </Fragment>
  );
}
