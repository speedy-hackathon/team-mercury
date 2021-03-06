import React from "react";
import styles from "./style.module.css";
import Field from "../Field";
import { DELAY, MAX_HEIGHT, MAX_WIDTH } from "../../consts/sizes";
import { gameStateUrl, userActionUrl } from "../../consts/urls";
import errorHandler from "../../utils/errorHandler";
import Instruction from "../Instruction";
import { restartGameRequest } from "../api/api";
import { StatsChart } from "../Chart/Chart";

import "./base.css";

export default class App extends React.Component {
  constructor() {
    super();
    this.state = {
      people: [],
      map: [],
      instructionOpen: true,
      allStatistic: [],
      isRadiusOn: false,
    };
    this.intervalId = null;
  }

  componentWillUnmount() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  changeRadiusStatus(value) {
    this.setState({isRadiusOn: value});
  }

  render() {
    const { people, map, instructionOpen, isRadiusOn} = this.state;
    return (
      <div className={styles.root}>
        <div className={styles.optionsPanel}>
          <button className={styles.restartGame} onClick={restartGameRequest}>Начать сначала</button>
          <label>Показать радиус заражения<input type="checkbox" onChange={(e) => this.changeRadiusStatus(e.currentTarget.checked)}/></label>
        </div>
        {instructionOpen && <Instruction onClose={this.closeInstruction} />}
        <h1 className={styles.title}>Симулятор COVID</h1>
        <Field map={map} people={people} onClick={this.personClick} />
        <StatsChart chartData={this.state.allStatistic} />
      </div>
    );
  }

  closeInstruction = () => {
    this.setState({
      instructionOpen: false,
    });

    this.getNewStateFromServer();

    this.intervalId = setInterval(this.getNewStateFromServer, DELAY);
  };

  personClick = (id) => {
    fetch(userActionUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        personClicked: id,
      }),
    }).then(errorHandler);
  };

  getNewStateFromServer = () => {
    fetch(gameStateUrl)
      .then(errorHandler)
      .then((res) => res.json())
      .then((game) => {
        this.setState({
          people: game.people,
          map: game.map.houses.map((i) => i.coordinates.leftTopCorner),
          allStatistic: [game.statistic, ...this.state.allStatistic]
        });
      });
  };
}
