import { React, useState } from "react";
import {
  Pressable,
  Text,
  View,
  StyleSheet,
  Image,
  TouchableOpacity,
} from "react-native";
import MockHunts from "../../mocks/mockHunts.json";
import { Colors } from "../../styles";

export default function TreasureHunts() {
  const [focusId, setFocusId] = useState("");

  function handleFocus(newFocusId) {
    if (focusId === newFocusId) {
      setFocusId("");
    } else {
      setFocusId(newFocusId);
    }
  }

  let hunts = MockHunts.map((hunt) => {
    return (
      <Pressable
        onPress={() => {
          handleFocus(hunt.huntId);
        }}
        style={[styles.huntBox, focusId === hunt.huntId && styles.inFocus]}
        key={hunt.huntId}
      >
        <View style={styles.hunt}>
          {hunt.image && (
            <Image
              style={styles.tinyLogo}
              source={{
                uri: "https://img.freepik.com/premium-photo/mountains-during-flowers-blossom-sunrise-flowers-mountain-hills-beautiful-natural-landscape-summer-time-mountainimage_647656-1502.jpg?w=2000",
              }}
            ></Image>
          )}
          <Text style={styles.huntElement}>{hunt.title}</Text>
        </View>
        {focusId === hunt.huntId && (
          <View style={styles.sideBySideChildren}>
            <TouchableOpacity style={styles.buttonMinimalistic}>
              <Text style={styles.buttonTextMinimalisticCenter}>Play</Text>
            </TouchableOpacity>
            <TouchableOpacity style={styles.buttonMinimalistic}>
              <Text style={styles.buttonTextMinimalisticCenter}>Edit</Text>
            </TouchableOpacity>
            <TouchableOpacity style={styles.buttonMinimalistic}>
              <Text style={styles.buttonTextMinimalisticCenter}>Delete</Text>
            </TouchableOpacity>
            <TouchableOpacity style={styles.buttonMinimalistic}>
              <Text style={styles.buttonTextMinimalisticCenter}>
                Participants
              </Text>
            </TouchableOpacity>
          </View>
        )}
      </Pressable>
    );
  });

  return (
    <View style={styles.huntList}>
      <View style={styles.listCategory}>
        <Text style={styles.listCategoryText}>Your Treasure Hunts</Text>
      </View>
      {hunts}//add reset option?
      <View style={styles.listCategory}>
        <Text style={styles.listCategoryText}>Shared With You</Text>
        //invites //dropdown is accept/decline, or play/reset
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  listCategoryText: {
    textAlign: "center",
    fontSize: 30,
  },
  listCategory: {},
  inFocus: {
    borderWidth: 2,
    borderColor: Colors.primaryDark,
  },
  buttonMinimalistic: {
    flex: 1,
    margin: 1,
    paddingVertical: 10,
    backgroundColor: Colors.primaryLight,
  },
  buttonTextMinimalisticCenter: {
    textAlign: "center",
    fontSize: 15,
  },
  sideBySideChildren: {
    flexDirection: "row",
    justifyContent: "space-evenly",
  },
  huntList: {
    flex: 1,
    backgroundColor: Colors.primaryLight,
  },
  huntBox: {
    width: "100%",
    marginTop: 5,
    marginBotton: 5,
    backgroundColor: Colors.primaryDark,
  },
  hunt: {
    position: "relative",
    height: 100,
    overflow: "hidden",
  },
  huntElement: {
    position: "absolute",
    width: "100%",
    fontSize: 25,
    textAlign: "center",
    textAlignVertical: "center",
  },
  tinyLogo: {
    position: "absolute",
    width: "100%",
    aspectRatio: 1,
  },
});
