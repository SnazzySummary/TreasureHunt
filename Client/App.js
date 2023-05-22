import { StatusBar } from "react-native";
import { useState } from "react";
import { NavigationContainer } from "@react-navigation/native";
import Login from "./components/Login";
import DrawerStackNavigator from "./components/DrawerStackNavigator";

export default function App() {
  const [loggedIn, setLoggedIn] = useState(false);

  return (
    <NavigationContainer>
      <StatusBar />
      {loggedIn ? (
        <DrawerStackNavigator />
      ) : (
        <Login setLoggedIn={setLoggedIn} />
      )}
    </NavigationContainer>
  );
}
