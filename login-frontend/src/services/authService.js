import axios from "axios";

const API_URL = "http://localhost:5139/api/Auth";

const api = axios.create({
    baseURL: API_URL,
    headers: {
        "Content-Type": "application/json"
    }
});

export const registerUser = async (userData) => {
    const response = await api.post("/register", userData);
    return response.data;
}

export const loginUser = async (credentials) => {
    const response = await api.post("/login", credentials);

    sessionStorage.setItem(
        "accessToken",
        response.data.accessToken
    );
    return response.data;
};

export const getUserProfile = async () => {
    const token = sessionStorage.getItem("accessToken");

    const response = await api.get("/profile", {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
};

export const logoutUser = () => {
    sessionStorage.removeItem("accessToken");
};