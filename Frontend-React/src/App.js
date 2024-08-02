import './App.css'
import { Image, Alert, Button, Container, Row, Col, Form, Table, Stack } from 'react-bootstrap'
import React, { useState, useEffect } from 'react'
import { TodoListApi } from './TodoListApi.js'


//const axios = require('axios')

const App = () => {
  const [description, setDescription] = useState('')
  const [items, setItems] = useState([])
  const [errors, setErrors] = useState([])

  let todoApi = new TodoListApi();

  useEffect(() => {
    setItems([]);
    getItems();
  }, [])

  const renderAddTodoItemContent = () => {
    return (
      <Container>
        
          { errors.length > 0 ?
            <>
            <h2 className="warning">Errors</h2>
            <table>
              <tbody>
                {errors.map((item) => (
                    <tr key={item.propertyName+item.errorMessage}>
                      <td>{item.propertyName}</td>
                      <td>{item.errorMessage}</td>
                    </tr>
                ))}
              </tbody>
            </table>
            </> : null 
          }
        
        <h1>Add Item</h1>
        <Form.Group as={Row} className="mb-3" controlId="formAddTodoItem">
          <Form.Label column sm="2">
            Description
          </Form.Label>
          <Col md="6">
            <Form.Control
              type="text"
              placeholder="Enter description..."
              value={description}
              onChange={handleDescriptionChange}
            />
          </Col>
        </Form.Group>
        <Form.Group as={Row} className="mb-3 offset-md-2" controlId="formAddTodoItem">
          <Stack direction="horizontal" gap={2}>
            <Button variant="primary" onClick={() => handleAdd(description)}>
              Add Item
            </Button>
            <Button variant="secondary" onClick={() => handleClear()}>
              Clear
            </Button>
          </Stack>
        </Form.Group>
      </Container>
    )
  }

  const renderTodoItemsContent = () => {
    return (
      <>
        <h1>
          Showing {items.length} Item(s){' '}
          {/*
             Automatically refreshes now..
            <Button variant="primary" className="pull-right" onClick={() => getItems()}>
            Refresh
          </Button> */}
        </h1>

        <Table striped bordered hover>
          <thead>
            <tr>
              <th>Id</th>
              <th>Description</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id}>
                <td>{item.id}</td>
                <td>{item.description}</td>
                <td>
                  <Button variant="warning" size="sm" onClick={() => handleMarkAsComplete(item)}>
                    Mark as completed
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      </>
    )
  }

  const handleDescriptionChange = (event) => {
    setDescription(event.target.value);
  }

  async function getItems() {

    try {
        var items = await todoApi.Get()
        setItems(items);
      } catch (errorResponse) {
        console.error(errorResponse);
        setErrors(errorResponse.errors);
        setItems([]);
      }
  }

  async function handleAdd(description) {
    try {
      setErrors([]);
      await todoApi.Post(description)

    } catch (errorResponse) {
      console.error(errorResponse);
      setErrors(errorResponse.errors);
    }

    await getItems();
  }

  function handleClear() {
    setDescription('')
  }

  async function handleMarkAsComplete(item) {
    try {
      await todoApi.MarkAsComplete(item.id);

    } catch (errorResponse) {
      console.error(errorResponse)
      setErrors(errorResponse.errors);
    }

    await getItems();
  }

  return (
    <div className="App">
      <Container>
        <Row>
          <Col>
            <Image src="clearPointLogo.png" fluid rounded />
          </Col>
        </Row>
        <Row>
          <Col>
            <Alert variant="success">
              <Alert.Heading>Todo List App</Alert.Heading>
              Welcome to the ClearPoint frontend technical test. We like to keep things simple, yet clean so your
              task(s) are as follows:
              <br />
              <br />
              <ol className="list-left">
                <li>Add the ability to add (POST) a Todo Item by calling the backend API</li>
                <li>
                  Display (GET) all the current Todo Items in the below grid and display them in any order you wish
                </li>
                <li>
                  Bonus points for completing the 'Mark as completed' button code for allowing users to update and mark
                  a specific Todo Item as completed and for displaying any relevant validation errors/ messages from the
                  API in the UI
                </li>
                <li>Feel free to add unit tests and refactor the component(s) as best you see fit</li>

                <li>The ability to surface any errors from the backend API in the UI</li>
                <li>The ability to mark an item in the to-do list as complete</li>
                <li>Add unit tests to cover the new functionality using a framework of your choice</li>
              </ol>
            </Alert>
          </Col>
        </Row>
        <Row>
          <Col>{renderAddTodoItemContent()}</Col>
        </Row>
        <br />
        <Row>
          <Col>{renderTodoItemsContent()}</Col>
        </Row>
      </Container>
      <footer className="page-footer font-small teal pt-4">
        <div className="footer-copyright text-center py-3">
          © 2021 Copyright:
          <a href="https://clearpoint.digital" target="_blank" rel="noreferrer">
            clearpoint.digital
          </a>
        </div>
      </footer>
    </div>
  )
}

export default App
