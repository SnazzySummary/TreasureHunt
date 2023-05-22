import { React, useState } from "react";
import { Colors } from "../styles";
import {
  StyleSheet,
  Text,
  View,
  Image,
  TextInput,
  Button,
  TouchableOpacity,
} from "react-native";
import { login } from "../services/AuthenticationService";

export default function Login({ setLoggedIn }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [testToken, setTestToken] = useState("");

  async function handleLogin() {
    // let response = await login(email, password);
    // setTestToken(response);
    setLoggedIn(true);
  }

  return (
    <View style={styles.container}>
      {/* <Image style={styles.image} source={require("./assets/log2.png")} /> */}

      <View style={styles.InputView}>
        <TextInput
          style={styles.TextInput}
          placeholder="Email."
          placeholderTextColor={Colors.placeholderText}
          onChangeText={(email) => setEmail(email)}
        />
      </View>
      <View style={styles.InputView}>
        <TextInput
          style={styles.TextInput}
          placeholder="Password."
          placeholderTextColor={Colors.placeholderText}
          secureTextEntry={true}
          onChangeText={(password) => setPassword(password)}
        />
      </View>
      <TouchableOpacity>
        <Text style={styles.forgot_button}>Forgot Password?</Text>
      </TouchableOpacity>
      <TouchableOpacity onPress={handleLogin} style={styles.loginBtn}>
        <Text style={styles.loginText}>LOGIN</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
    alignItems: "center",
    justifyContent: "center",
  },
  image: {
    marginBottom: 40,
  },
  InputView: {
    height: 50,
    width: "70%",
    marginBottom: 20,
  },
  TextInput: {
    backgroundColor: Colors.primaryLight,
    borderRadius: 30,
    alignItems: "center",
    flex: 1,
    padding: 10,
  },
  forgot_button: {
    height: 30,
    marginBottom: 30,
  },
  loginBtn: {
    width: "80%",
    borderRadius: 25,
    height: 50,
    alignItems: "center",
    justifyContent: "center",
    marginTop: 40,
    backgroundColor: Colors.primaryDark,
  },
});
