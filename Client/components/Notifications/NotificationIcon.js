import React from "react";
import { Text, Pressable } from "react-native";

export default function NotificationIcon({ navigationAction }) {
  return (
    <Pressable onPress={navigationAction}>
      <Text>Notifications</Text>
    </Pressable>
  );
}
