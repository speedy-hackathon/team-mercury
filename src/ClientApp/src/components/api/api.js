import {gameRestartUrl} from "../../consts/urls";

export const restartGameRequest = async () => {
    const response = await fetch(gameRestartUrl, {method:"POST"});
    return response;
}
