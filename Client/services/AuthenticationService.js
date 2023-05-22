const BASE_URL = "https://10.0.0.218:5007/api";

export async function login(username, password) {
  console.log("Gonna Send to:" + BASE_URL + "/auth");
  try {
    // const response = await fetch(BASE_URL + "/auth", {
    //   method: "POST",
    //   headers: { "Content-Type": "application/json" },
    //   body: {
    //     user: username,
    //     password: password,
    //   },
    // });
    const response = await fetch(BASE_URL + "/HttpTrigger1", {
      method: "GET",
    });
    return await response.json();
  } catch (error) {
    console.log(error);
    return JSON.stringify("I fucked up.");
  }
}
